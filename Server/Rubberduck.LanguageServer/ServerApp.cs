using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nerdbank.Streams;
using NLog;
using NLog.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.General;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.WorkDone;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using OmniSharp.Extensions.LanguageServer.Server;
using Rubberduck.InternalApi.Common;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Workspace;
using Rubberduck.InternalApi.Settings;
using Rubberduck.LanguageServer.Handlers;
using Rubberduck.LanguageServer.Handlers.Lifecycle;
using Rubberduck.LanguageServer.Handlers.Workspace;
using Rubberduck.LanguageServer.Model;
using Rubberduck.LanguageServer.Services;
using Rubberduck.ServerPlatform;
using Rubberduck.SettingsProvider;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.SettingsProvider.Model.LanguageClient;
using Rubberduck.SettingsProvider.Model.LanguageServer;
using Rubberduck.SettingsProvider.Model.ServerStartup;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO.Abstractions;
using System.IO.Pipelines;
using System.IO.Pipes;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using OmniSharpLanguageServer = OmniSharp.Extensions.LanguageServer.Server.LanguageServer;

namespace Rubberduck.LanguageServer
{
    public partial class WorkDoneProgressParams : IWorkDoneProgressParams
    {

    }

    public sealed class ServerApp : IDisposable
    {
        private readonly ServerStartupOptions _options;
        private readonly CancellationTokenSource _tokenSource;

        private IDisposable _pipeWaitForClientConnectionTask;
        private NamedPipeServerStream? _pipe;

        private ILogger<ServerApp>? _logger;
        private IServiceProvider? _serviceProvider;

        private OmniSharpLanguageServer _languageServer = default!;
        private LanguageServerState _serverState = default!;

        public ServerApp(ServerStartupOptions options, CancellationTokenSource tokenSource)
        {
            _options = options;
            _tokenSource = tokenSource;
        }

        /// <summary>
        /// Configures and starts the language server, and then awaits an Exit notification.
        /// </summary>
        public async Task RunAsync()
        {
            try
            {
                var services = new ServiceCollection();
                services.AddLogging(ConfigureLogging);
                ConfigureServices(services);

                _serviceProvider = services.BuildServiceProvider();
                _logger = _serviceProvider.GetRequiredService<ILogger<ServerApp>>();
                _serverState = _serviceProvider.GetRequiredService<LanguageServerState>();

                _languageServer = await OmniSharpLanguageServer.From(ConfigureServer, _serviceProvider, _tokenSource.Token);
                
                await _languageServer.WaitForExit;
            }
            catch (OperationCanceledException)
            {
                _logger?.LogTrace("Token was cancelled; server process will exit normally.");
            }
            catch (Exception exception)
            {
                _logger?.LogError(exception, "An exception was thrown; server process will exit with an error code.");
                _logger?.LogCritical(exception.ToString());
                throw;
            }
        }

        private void ConfigureServices(IServiceCollection services)
        {
            //services.AddSingleton<ILanguageServerFacade>(provider => _languageServer);
            services.AddSingleton<ServerStartupOptions>(provider => _options);
            services.AddSingleton<Process>(provider => Process.GetProcessById((int)_options.ClientProcessId));

            services.AddSingleton<IFileSystem, FileSystem>();
            services.AddSingleton<PerformanceRecordAggregator>();

            services.AddSingleton<ServerPlatformServiceHelper>();
            services.AddSingleton<LanguageServerState>();
            services.AddSingleton<Func<LanguageServerState>>(provider => () => _serverState);
            services.AddSingleton<IServerStateWriter>(provider => provider.GetRequiredService<LanguageServerState>());

            services.AddSingleton<Func<LanguageClientStartupSettings>>(provider => () => provider.GetRequiredService<RubberduckSettingsProvider>().Settings.LanguageClientSettings.StartupSettings);
            services.AddSingleton<Func<LanguageServerStartupSettings>>(provider => () => provider.GetRequiredService<RubberduckSettingsProvider>().Settings.LanguageServerSettings.StartupSettings);
            services.AddSingleton<IDefaultSettingsProvider<RubberduckSettings>>(provider => RubberduckSettings.Default);
            services.AddSingleton<IDefaultSettingsProvider<LanguageServerSettings>>(provider => LanguageServerSettings.Default);
            services.AddSingleton<RubberduckSettingsProvider>();

            services.AddSingleton<SupportedLanguage, VisualBasicForApplicationsLanguage>();
            services.AddSingleton<DocumentContentStore>();

            services.AddSingleton<IExitHandler, ExitHandler>();
            services.AddSingleton<IHealthCheckService<LanguageServerStartupSettings>, ClientProcessHealthCheckService<LanguageServerStartupSettings>>();

            services.AddSingleton<IWorkDoneProgressStateService, WorkDoneProgressStateService>();
            services.AddSingleton<ISettingsChangedHandler<RubberduckSettings>>(provider => provider.GetRequiredService<RubberduckSettingsProvider>());
            services.AddSingleton<DidChangeConfigurationHandler>();
        }

        private void ConfigureLogging(ILoggingBuilder builder)
        {
            builder.AddNLog(provider =>
            {
                var factory = new LogFactory
                {
                    AutoShutdown = true,
                    ThrowConfigExceptions = true,
                    ThrowExceptions = true,
                    DefaultCultureInfo = CultureInfo.InvariantCulture,
                    GlobalThreshold = NLog.LogLevel.Trace,
                };
                factory.Setup(builder =>
                {
                    builder.LoadConfigurationFromFile("NLog-server.config");
                });

                return factory;
            });
        }

        private void ConfigureServer(LanguageServerOptions options)
        {
            // example LSP server: https://github.com/OmniSharp/csharp-language-server-protocol/blob/master/sample/SampleServer/Program.cs

            ConfigureTransport(options);

            var assembly = GetType().Assembly.GetName();
            var name = assembly.Name ?? throw new InvalidOperationException();
            var version = assembly.Version?.ToString(3);

            var info = new ServerInfo
            {
                Name = name,
                Version = version
            };

            var loggerProvider = new NLogLoggerProvider();
            loggerProvider.LogFactory.Setup().LoadConfigurationFromFile("NLog-server.config");
            options.LoggerFactory = new NLogLoggerFactory(loggerProvider);

            options.WithUnhandledExceptionHandler(exception =>
            {
                _logger?.LogError("{0}", exception.ToString());
            });

            _ = options
                .WithServerInfo(info)

                .OnInitialize(async (ILanguageServer server, InitializeParams request, CancellationToken token) =>
                {
                    // OnLanguageServerInitializeDelegate
                    // Gives your class or handler an opportunity to interact with the InitializeParams
                    // before it is processed by the server.

                    _logger?.LogDebug("Received Initialize request.");
                    token.ThrowIfCancellationRequested();
                    if (TimedAction.TryRun(() =>
                    {                        
                        // we cannot request a progress token during initialization
                        // we can only report progress if a token was provided here.
                        var initProgressToken = request.WorkDoneToken;
                        
                        var progress = initProgressToken is null ? null
                            : server.WorkDoneManager.For(new WorkDoneProgressParams { WorkDoneToken = initProgressToken },
                                new WorkDoneProgressBegin 
                                {
                                    Title = "Initializing",
                                    Message = "Initialization started.",
                                    Cancellable = false
                                },
                                onError:(exception) => new WorkDoneProgressEnd
                                {
                                    Message = request.Trace == InitializeTrace.Verbose ? exception.ToString() : exception.Message
                                },
                                onComplete:() => new WorkDoneProgressEnd
                                {
                                    Message = "Completed."
                                });
                        try
                        {
                            progress?.OnNext("Processing initialization parameters...", null, null);

                            if (request.ProcessId != _options.ClientProcessId)
                            {
                                throw new InvalidOperationException($"Request ProcessId={request.ProcessId} mismatched expected ClientProcessId={_options.ClientProcessId}.");
                            }
                            _serverState.Initialize(request);
                            
                            progress?.OnNext("Validating workspace root...", null, null);
                            var workspaceRoot = new System.IO.DirectoryInfo(_serverState.RootUri?.GetFileSystemPath() ?? request.RootPath ?? throw new InvalidOperationException("Workspace root URI was not supplied."));
                            if (!workspaceRoot.Exists)
                            {
                                throw new InvalidOperationException("Specified workspace does not exist.");
                            }

                            _logger?.LogTrace(TraceLevel.Verbose, "Validated workspace root.", workspaceRoot.FullName);
                            progress?.OnCompleted();
                        }
                        catch (Exception exception)
                        {
                            _logger?.LogError(exception, exception.ToString());
                            progress?.OnError(exception);
                            throw;
                        }

                    }, out var elapsed, out var exception))
                    {
                        _logger?.LogPerformance(TraceLevel.Verbose, "Handling initialize...", elapsed);
                    }
                    else if (exception != null)
                    {
                        _logger?.LogError(TraceLevel.Verbose, exception);
                        throw exception; // TODO throw new LspInitFailedException(message, exception)
                    }
                })

                .OnInitialized(async (ILanguageServer server, InitializeParams request, InitializeResult response, CancellationToken cancellationToken) =>
                {
                    // OnLanguageServerInitializedDelegate
                    // Gives your class or handler an opportunity to interact with the InitializeParams and InitializeResult
                    // after it is processed by the server but before it is sent to the client.

                    _logger?.LogTrace("Sending Initialized notification. InitializeResult: {0}", JsonSerializer.Serialize(response));
                })

                .OnStarted(async (ILanguageServer server, CancellationToken token) =>
                {
                    // OnLanguageServerStartedDelegate
                    // Gives your class or handler an opportunity to interact with the ILanguageServer
                    // after the connection has been established.

                    _logger?.LogDebug("Language server started.");
                    token.ThrowIfCancellationRequested();

                    try
                    {
                        var handlerProgressToken = new ProgressToken(Guid.NewGuid().ToString());
                        IWorkDoneObserver? progress = null;

                        if (server.WorkDoneManager.IsSupported)
                        {
                            _logger?.LogTrace("Sending progress token...");
                            server.Window.SendWorkDoneProgressCreate(new() { Token = handlerProgressToken });

                            progress = await server.WorkDoneManager.Create(
                                new WorkDoneProgressBegin
                                {
                                    Title = "Processing workspace",
                                    Message = "Processing workspace...",
                                    Cancellable = true,
                                    // LSP: if not provided, infinite progress is assumed
                                    // and clients are allowed to ignore the 'percentage' value in subsequent report notifications
                                    Percentage = 0,
                                },
                                onError: (exception) => new WorkDoneProgressEnd { Message = exception.Message },
                                onComplete: () => new WorkDoneProgressEnd { Message = "Completed." }, token);
                        }
                        else
                        {
                            _logger?.LogTrace("WorkDoneManager.IsSupported returned false; client will not be sent progress notifications.");
                        }

                        progress?.OnNext(message: "Loading source files...", percentage: 10, cancellable: false);

                        var rootUri = _serverState.RootUri!.GetFileSystemPath();
                        var store = server.Services.GetRequiredService<DocumentContentStore>();

                        var projectFilePath = System.IO.Path.Combine(rootUri, ProjectFile.FileName);
                        var projectFileContent = System.IO.File.ReadAllText(projectFilePath);
                        var projectFile = JsonSerializer.Deserialize<ProjectFile>(projectFileContent) ?? throw new InvalidOperationException("Project file could not be deserialized.");

                        var srcRootPath = System.IO.Path.Combine(rootUri, ProjectFile.SourceRoot);
                        foreach (var item in projectFile.VBProject.Modules)
                        {
                            progress?.OnNext($"Loading file: {item.Name}", 10, false);

                            var path = System.IO.Path.Combine(srcRootPath, item.Uri);
                            var state = new DocumentState(System.IO.File.ReadAllText(path));
                            state.IsOpened = item.IsAutoOpen;
                            store.AddOrUpdate(new Uri(path), state);
                            _logger?.LogTrace("Loaded source file: {path}", path);
                        }

                        progress?.OnNext($"Loading project references...", percentage: 20, cancellable: false);
                        foreach (var item in projectFile.VBProject.References)
                        {
                            progress?.OnNext($"Loading library: {item.Name}", 20, false);
                            _logger?.LogTrace("todo: load definitions from {uri} ({typeLibInfoUri})", item.Uri, item.TypeLibInfoUri);
                        }

                        progress?.OnNext(message: "Parsing...", percentage: 25, cancellable: false);
                        // todo: parse all source files in workspace, make code foldings immediately available for open documents

                        progress?.OnNext(message: "Resolving...", percentage: 50, cancellable: false);
                        // todo: make semantic highlighting immediately available for open documents

                        progress?.OnNext(message: "Analyzing...", percentage: 75, cancellable: false);
                        // todo: make diagnostics immediately available for open documents

                        progress?.OnCompleted();
                        _logger?.LogTrace("WorkDoneToken '{0}' has been completed.", progress?.WorkDoneToken);
                        _logger?.LogDebug("Handled Started notification.");
                    }
                    catch (Exception exception)
                    {
                        _logger?.LogError(exception, "{0}", exception.ToString());
                    }
                })

                 /* handlers */

                 //.WithHandler<SetTraceHandler>()

                 /* registrations */

                 .WithHandler<ShutdownHandler>()
                 .WithHandler<ExitHandler>()
            // .WithHandler<InitializedHandler>()

                .OnDidOpenTextDocument(HandleDidOpenTextDocument, GetTextDocumentOpenRegistrationOptions)

            /*/ Workspace
                .WithHandler<DidChangeConfigurationHandler>()
                .WithHandler<DidChangeWatchedFileHandler>()
                .WithHandler<DidChangeWorkspaceFoldersHandler>()
                .WithHandler<DidCreateFileHandler>()
                .WithHandler<DidDeleteFileHandler>()
                .WithHandler<DidRenameFileHandler>()
                .WithHandler<ExecuteCommandHandler>()
                .WithHandler<SymbolInformationHandler>()
                .WithHandler<WorkspaceDiagnosticHandler>()
                .WithHandler<WorkspaceSymbolResolveHandler>()
                .WithHandler<WorkspaceSymbolsHandler>()
            */

            /*/ TextDocument
                .WithHandler<CallHierarchyHandler>()
                .WithHandler<CodeActionHandler>()
                .WithHandler<CodeActionResolveHandler>()
                .WithHandler<CodeLensHandler>()
                .WithHandler<ColorPresentationHandler>()
                .WithHandler<CompletionHandler>()
                .WithHandler<DeclarationHandler>()
                .WithHandler<DefinitionHandler>()
                .WithHandler<DocumentColorHandler>()
                .WithHandler<DocumentDiagnosticHandler>()
                .WithHandler<DocumentFormattingHandler>()
                .WithHandler<DocumentHighlightHandler>()
                .WithHandler<DocumentOnTypeFormattingHandler>()
                .WithHandler<DocumentRangeFormattingHandler>()
                .WithHandler<DocumentSymbolHandler>()
                .WithHandler<FoldingRangeHandler>()
                .WithHandler<HoverHandler>()
                .WithHandler<ImplementationHandler>()
                .WithHandler<PrepareRenameHandler>()
                .WithHandler<ReferencesHandler>()
                .WithHandler<RenameHandler>()
                .WithHandler<SelectionRangeHandler>()
                .WithHandler<SemanticTokensHandler>()
                .WithHandler<SemanticTokensFullHandler>()
                .WithHandler<SignatureHelpHandler>()
                .WithHandler<TextDocumentSyncHandler>()
                .WithHandler<TypeDefinitionHandler>()
                .WithHandler<TypeHierarchyHandler>()
            */
            ;
        }

        private TextDocumentSelector GetSelector(SupportedLanguage language)
        {
            var filter = new TextDocumentFilter
            {
                Language = language.Id,
                Pattern = string.Join(";", language.FileTypes.Select(fileType => $"**/{fileType}").ToArray())
            };
            return new TextDocumentSelector(filter);
        }

        private void HandleDidOpenTextDocument(DidOpenTextDocumentParams request, TextSynchronizationCapability capability, CancellationToken token)
        {
            _logger?.LogDebug("Received DidOpenTextDocument notification.");
        }

        private TextDocumentOpenRegistrationOptions GetTextDocumentOpenRegistrationOptions(TextSynchronizationCapability capability, ClientCapabilities clientCapabilities)
        {
            return new TextDocumentOpenRegistrationOptions
            {
                DocumentSelector = GetSelector(new VisualBasicForApplicationsLanguage())
            };
        }

        private void ConfigureTransport(LanguageServerOptions options)
        {
            switch (_options.TransportType)
            {
                case TransportType.StdIO:
                    ConfigureStdIO(options);
                    break;

                case TransportType.Pipe:
                    ConfigurePipeIO(options);
                    break;

                default:
                    _logger?.LogWarning("An unsupported transport type was specified.");
                    throw new UnsupportedTransportTypeException(_options.TransportType);
            }
        }

        private void ConfigureStdIO(LanguageServerOptions options)
        {
            _logger?.LogInformation($"Configuring language server transport to use standard input/output streams...");

            options.WithInput(Console.OpenStandardInput());
            options.WithOutput(Console.OpenStandardOutput());
        }

        private void ConfigurePipeIO(LanguageServerOptions options)
        {
            var ioOptions = (PipeServerStartupOptions)_options;
            _logger?.LogInformation("Configuring language server transport to use a named pipe stream (name: {name})...", ioOptions.PipeName);

            _pipe = new NamedPipeServerStream(ioOptions.PipeName, PipeDirection.InOut, 10, PipeTransmissionMode.Message, System.IO.Pipes.PipeOptions.CurrentUserOnly | System.IO.Pipes.PipeOptions.Asynchronous | System.IO.Pipes.PipeOptions.FirstPipeInstance);
            options.WithInput(PipeReader.Create(_pipe));
            options.WithOutput(PipeWriter.Create(_pipe));

            _logger?.LogTrace("Asynchronously awaiting client connection...");
            _pipeWaitForClientConnectionTask = _pipe.WaitForConnectionAsync();
        }

        public void Dispose()
        {
            if (_pipe is not null)
            {
                _logger?.LogDebug("Disposing NamedPipeServerStream...");
                _pipeWaitForClientConnectionTask.Dispose();
                _pipe.Dispose();
                _pipe = null;
            }

            if (_languageServer is not null)
            {
                _logger?.LogDebug("Disposing LanguageServer...");
                _languageServer.Dispose();
                _languageServer = null!;
            }
        }
    }
}