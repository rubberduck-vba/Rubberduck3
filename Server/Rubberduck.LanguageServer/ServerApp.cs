using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nerdbank.Streams;
using NLog.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.General;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Server;
using Rubberduck.InternalApi.Common;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.ServerPlatform;
using Rubberduck.LanguageServer.Handlers;
using Rubberduck.LanguageServer.Model;
using Rubberduck.LanguageServer.Services;
using Rubberduck.ServerPlatform;
using Rubberduck.SettingsProvider;
using Rubberduck.SettingsProvider.Model;
using System;
using System.Diagnostics;
using System.IO.Pipes;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using OmniSharpLanguageServer = OmniSharp.Extensions.LanguageServer.Server.LanguageServer;

namespace Rubberduck.LanguageServer
{
    public class ServerStateNotInitializedException : InvalidOperationException { }

    public sealed class ServerApp : IDisposable
    {
        private readonly ServerStartupOptions _options;
        private readonly CancellationTokenSource _tokenSource;

        private NamedPipeServerStream? _pipe;

        private ILogger<ServerApp> _logger = default!;
        private IHealthCheckService _healthCheckService = default;
        private IServiceScope _serviceScope = default!;

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
            var services = new ServiceCollection();
            services.AddLogging(ConfigureLogging);

            var provider = new DefaultServiceProviderFactory().CreateServiceProvider(services);
            var scope = provider.CreateScope();

            _logger = scope.ServiceProvider.GetRequiredService<ILogger<ServerApp>>();
            LogPreInitialization();

            _serverState = new(scope.ServiceProvider.GetRequiredService<ILogger<LanguageServerState>>(), _options);

            try
            {
                _languageServer = await OmniSharpLanguageServer.From(options => ConfigureLanguageServer(options), provider, _tokenSource.Token);
                await _languageServer.WaitForExit;
            }
            catch (OperationCanceledException) 
            {
                _logger.LogTrace("Token was cancelled; server process will exit normally.");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An exception was thrown; server process will exit with an error code.");
                throw;
            }
        }

        private void LogPreInitialization()
        {
            _logger.LogInformation("Rubberduck 3.0 Language Server v{version}", Assembly.GetExecutingAssembly()?.GetName()?.Version?.ToString(3) ?? "0.x");
            _logger.LogDebug("Startup options:\n{options}'", _options);
            _logger.LogInformation("Starting language server...");
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<LanguageServerState>();
            services.AddScoped<IServiceProvider>(provider => provider);
            services.AddScoped<IHealthCheckService>(provider => new ClientProcessHealthCheckService(
                logger: provider.GetRequiredService<ILogger<ClientProcessHealthCheckService>>(),
                settingsProvider: provider.GetRequiredService<ISettingsProvider<LanguageServerSettings>>(),
                process: Process.GetProcessById((int)_serverState.ClientProcessId),
                server: _languageServer));

            services.AddScoped<ServerStartupOptions>(provider => _options);
            services.AddScoped<IServerStateWriter>(provider => provider.GetRequiredService<LanguageServerState>());

            services.AddScoped<IDefaultSettingsProvider<LanguageServerSettings>>(provider => LanguageServerSettings.Default);
            services.AddScoped<ISettingsProvider<LanguageServerSettings>, SettingsService<LanguageServerSettings>>();
            services.AddScoped<SupportedLanguage, VisualBasicForApplicationsLanguage>();
            services.AddScoped<DocumentContentStore>();
        
            _serviceScope = new DefaultServiceProviderFactory().CreateServiceProvider(services).CreateScope();
        }

        private void ConfigureLogging(ILoggingBuilder builder)
        {
            builder.AddNLog("NLog-server.config");
        }

        private void ConfigureLanguageServer(LanguageServerOptions options)
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

            options
                .WithServerInfo(info)
                .WithServices(ConfigureServices)

                .OnInitialize((ILanguageServer server, InitializeParams request, CancellationToken token) =>
                {
                    _logger.LogDebug("Received Initialize request.");
                    token.ThrowIfCancellationRequested();
                    if (TimedAction.TryRun(() =>
                    {
                        _serverState.Initialize(request);
                    }, out var elapsed, out var exception))
                    {
                        _logger.LogPerformance(TraceLevel.Verbose, "Handling initialize...", elapsed);
                    }
                    else if (exception != null)
                    {
                        _logger.LogError(TraceLevel.Verbose, exception);
                    }
                    _logger.LogDebug("Completed Initialize request.");
                    return Task.CompletedTask;
                })

                .OnInitialized((ILanguageServer server, InitializeParams request, InitializeResult response, CancellationToken token) =>
                {
                    _logger.LogDebug("Received Initialized notification.");
                    token.ThrowIfCancellationRequested();

                    _logger.LogDebug("Handled OnInitialized notification.");
                    return Task.CompletedTask;
                })

                .OnStarted((ILanguageServer server, CancellationToken token) =>
                {
                    _logger.LogDebug("Language server started.");
                    token.ThrowIfCancellationRequested();

                    if (TimedAction.TryRun(() =>
                    {
                        _healthCheckService = _serviceScope.ServiceProvider.GetRequiredService<IHealthCheckService>();
                        _healthCheckService.Start();
                    }, out var elapsed, out var exception))
                    {
                        _logger.LogPerformance(_serverState.TraceLevel.ToTraceLevel(), "Started healthcheck service.", elapsed);
                    }
                    else if (exception != null)
                    {
                        _logger.LogError(_serverState.TraceLevel.ToTraceLevel(), exception, "Healthcheck service could not be started.");
                    }

                    _logger.LogDebug("Handled OnStarted notification.");
                    return Task.CompletedTask;
                })

                /* handlers */
                .WithHandler<SetTraceHandler>()

                /* registrations */

                .OnShutdown((ShutdownParams request, CancellationToken token) =>
                {
                    _logger.LogDebug("Received Shutdown notification.");
                    token.ThrowIfCancellationRequested();

                    _logger.LogDebug("Handled Shutdown notification; awaiting Exit notification...");
                    return Task.CompletedTask;
                })
                .OnExit(async (ExitParams request, CancellationToken token) =>
                {
                    _logger.LogDebug("Received Exit notification. Process will now be terminated.");
                    await Task.Delay(TimeSpan.FromMilliseconds(100), token).ConfigureAwait(false); // allow the logger to write that last entry.
                    
                    Environment.Exit(0); // FIXME stack should unwind all the way to the entry point... unclear how to do this. cancelling the _tokenSource throws, but doesn't bubble up.
                })

                //.WithHandler<InitializedHandler>()

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

        private void ConfigureTransport(LanguageServerOptions options)
        {
            switch (_options.TransportType)
            {
                case TransportType.StdIO:
                    ConfigureStdIO(options);
                    break;

                case TransportType.Pipe:
                    ConfigurePipe(options);
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

        private void ConfigurePipe(LanguageServerOptions options)
        {
            var ioOptions = (PipeServerStartupOptions)_options;
            _logger?.LogInformation("Configuring language server transport to use a named pipe stream (mode: {mode})...", ioOptions.Mode);

            _pipe = new NamedPipeServerStream(ioOptions.PipeName, PipeDirection.InOut, 1, ioOptions.Mode, PipeOptions.Asynchronous);
            options.WithInput(_pipe.UsePipeReader());
            options.WithOutput(_pipe.UsePipeWriter());
        }

        public void Dispose()
        {
            if (_pipe is not null)
            {
                _logger?.LogDebug("Disposing NamedPipeServerStream...");
                _pipe.Dispose();
                _pipe = null;
            }

            if (_languageServer is not null)
            {
                _logger?.LogDebug("Disposing LanguageServer...");
                _languageServer.Dispose();
                _languageServer = null!;
            }

            if (_serviceScope is not null)
            {
                _logger?.LogDebug("Disposing service scope...");
                _serviceScope.Dispose();
                _serviceScope = null!;
            }
        }

        public class UnsupportedTransportTypeException : NotSupportedException
        {
            public UnsupportedTransportTypeException(TransportType transportType)
                : base($"Transport type '{transportType}' is not supported.") { }

            public TransportType UnsupportedTransportType { get; }
        }
    }
}