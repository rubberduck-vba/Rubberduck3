using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nerdbank.Streams;
using NLog.Extensions.Logging;
using System;
using System.IO.Pipes;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Server;
using Rubberduck.LanguageServer.Handlers;
using Rubberduck.LanguageServer.Services;
using Rubberduck.LanguageServer.Model;
using Rubberduck.InternalApi.ServerPlatform;
using Rubberduck.ServerPlatform;
using System.Globalization;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using System.Linq;
using System.Collections.Generic;
using OmniSharpLanguageServer = OmniSharp.Extensions.LanguageServer.Server.LanguageServer;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using Newtonsoft.Json.Linq;
using System.Reactive;
using System.Linq.Expressions;

namespace Rubberduck.LanguageServer
{
    public interface IServerStateWriter
    {
        void Initialize(InitializeParams param);
    }

    public class InvalidInitializeParamsException : ArgumentException
    {
        public static void ThrowIfNull(InitializeParams param, params Func<InitializeParams, (string, object?)>[] requiredValues)
        {
            foreach (var requiredValue in requiredValues)
            {
                var (property, value) = requiredValue.Invoke(param);
                if (value is null)
                {
                    throw new InvalidInitializeParamsException(property);
                }
            }
        }

        private InvalidInitializeParamsException(string property)
            : base($"Property '{property}' cannot be null") { }
    }

    public class ServerStateNotInitializedException : InvalidOperationException { }

    public class LanguageServerState : LanguageServerState<object>
    {
        public LanguageServerState(ILogger<LanguageServerState> logger) : base(logger) { }
    }

    public class LanguageServerState<TOptions> : IServerStateWriter
        where TOptions : new()
    {
        public LanguageServerState(ILogger logger)
        {
            _logger = logger;

            _clientInfo = default;
            _capabilities = default;
            _processId = default;
            _locale = default;
            _workspaceFolders = default;
            _options = default;
        }

        private readonly ILogger _logger;
        protected ILogger Logger => _logger;

        private ClientInfo? _clientInfo;
        public ClientInfo ClientInfo => _clientInfo ?? throw new ServerStateNotInitializedException();

        private ClientCapabilities? _capabilities;
        public ClientCapabilities ClientCapabilities => _capabilities ?? throw new ServerStateNotInitializedException();

        private long? _processId;
        public long ClientProcessId => _processId ?? throw new ServerStateNotInitializedException();

        private TOptions? _options;
        public TOptions Options => _options ?? throw new ServerStateNotInitializedException();

        private CultureInfo? _locale;
        public CultureInfo Locale => _locale ?? throw new ServerStateNotInitializedException();

        private InitializeTrace? _traceLevel;
        public InitializeTrace TraceLevel => _traceLevel ?? throw new ServerStateNotInitializedException();

        private Container<WorkspaceFolder>? _workspaceFolders;
        public IEnumerable<WorkspaceFolder> Workspacefolders => _workspaceFolders ?? throw new ServerStateNotInitializedException();

        public void Initialize(InitializeParams param)
        {
            InvalidInitializeParamsException.ThrowIfNull(param,
                e => (nameof(e.ClientInfo), e.ClientInfo),
                e => (nameof(e.Capabilities), e.Capabilities),
                //e => (nameof(e.Locale), e.Locale),
                //e => (nameof(e.ProcessId), e.ProcessId),
                e => (nameof(e.WorkspaceFolders), e.WorkspaceFolders)
            );

            _capabilities = param.Capabilities!;
            _clientInfo = param.ClientInfo!;
            //_locale = ToCultureInfo(param.Locale!);
            //_processId = param.ProcessId!.Value;
            _traceLevel = param.Trace!;
            _workspaceFolders = param.WorkspaceFolders!;

            _options = (TOptions)(param.InitializationOptions ?? new());
        }

        private CultureInfo ToCultureInfo(string? locale)
        {
            try
            {
                if (locale is null)
                {
                    _logger.LogWarning("Could not set locale from initialization parameters.");
                    return CultureInfo.InvariantCulture;
                }

                return CultureInfo.GetCultureInfo(locale);
            }
            catch
            {
                _logger.LogWarning("Could not set locale from initialization parameters.");
                return CultureInfo.InvariantCulture;
            }
        }
    }

    public sealed class ServerApp : IDisposable
    {
        private readonly TransportOptions _options;
        private readonly CancellationTokenSource _tokenSource;

        private NamedPipeServerStream? _pipe;

        private ILogger<ServerApp> _logger = default!;
        private OmniSharpLanguageServer _languageServer = default!;
        private LanguageServerState _serverState = default!;

        public ServerApp(TransportOptions options, CancellationTokenSource tokenSource)
        {
            _options = options;
            _tokenSource = tokenSource;
        }

        public async Task RunAsync()
        {
            var services = new ServiceCollection();
            services.AddLogging(ConfigureLogging);

            var provider = new DefaultServiceProviderFactory().CreateServiceProvider(services);
            using var scope = provider.CreateScope();
            
            _logger = scope.ServiceProvider.GetRequiredService<ILogger<ServerApp>>();
            _logger.LogInformation("Rubberduck 3.0 Language Server v{version}", Assembly.GetExecutingAssembly()?.GetName()?.Version?.ToString(3) ?? "0.x");
            _logger.LogInformation("Starting language server...");
            _logger.LogDebug("Startup configuration:: TraceLevel='{traceLevel}' TransportType='{transportType}'", _options.TraceLevel, _options.TransportType);

            _serverState = new(scope.ServiceProvider.GetRequiredService<ILogger<LanguageServerState>>());
            _languageServer = await OmniSharpLanguageServer.From(options => ConfigureLanguageServer(options), provider, _tokenSource.Token);

            _logger.LogInformation("Awaiting client connection...");
            
            await _languageServer.WaitForExit;
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<LanguageServerState>();
            services.AddSingleton<IServerStateWriter>(provider => provider.GetRequiredService<LanguageServerState>());

            

            services.AddScoped<SupportedLanguage, VisualBasicForApplicationsLanguage>();
            services.AddScoped<DocumentContentStore>();
        }

        private void ConfigureLogging(ILoggingBuilder builder)
        {
            builder.AddNLog("NLog.config"); // mind: release would deploy all executables to a single folder, so NLog.config is shared!
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
                    _logger.LogInformation("Received Initialize request.");
                    token.ThrowIfCancellationRequested();

                    _serverState.Initialize(request);
                    _logger.LogInformation("Completed Initialize request.");
                    return Task.CompletedTask;
                })

                .OnInitialized((ILanguageServer server, InitializeParams request, InitializeResult response, CancellationToken token) =>
                {
                    _logger.LogInformation("Received Initialized notification.");
                    _logger.LogInformation($"Token Cancelled: {token.IsCancellationRequested}");
                    token.ThrowIfCancellationRequested();

                    _logger.LogInformation("Completed OnInitialized request.");
                    return Task.CompletedTask;
                })

                .OnStarted((ILanguageServer server, CancellationToken token) =>
                {
                    _logger.LogInformation("Language server started.");
                    token.ThrowIfCancellationRequested();

                    // TODO start synchronizing documents

                    _logger.LogInformation("Completed OnStarted request.");
                    return Task.CompletedTask;
                })


                //.WithHandler<InitializedHandler>()
                .WithHandler<ShutdownHandler>()
                .WithHandler<ExitHandler>()
                .WithHandler<SetTraceHandler>()

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
            var ioOptions = (PipeTransportOptions)_options;
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
        }

        public class UnsupportedTransportTypeException : NotSupportedException
        {
            public UnsupportedTransportTypeException(TransportType transportType)
                : base($"Transport type '{transportType}' is not supported.") { }

            public TransportType UnsupportedTransportType { get; }
        }
    }
}