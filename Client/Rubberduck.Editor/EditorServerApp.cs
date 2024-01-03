using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using OmniSharp.Extensions.LanguageServer.Server;
using Rubberduck.Editor.RPC.EditorServer;
using Rubberduck.Editor.RPC.EditorServer.Handlers.Lifecycle;
using Rubberduck.Editor.RPC.EditorServer.Handlers.Workspace;
using Rubberduck.InternalApi.Common;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.ServerPlatform;
using Rubberduck.SettingsProvider.Model.ServerStartup;
using System;
using System.Diagnostics;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using OmniSharpLanguageServer = OmniSharp.Extensions.LanguageServer.Server.LanguageServer;

namespace Rubberduck.Editor
{
    /// <summary>
    /// Configures LSP for editor <--> addin communications.
    /// </summary>
    public sealed class EditorServerApp : IDisposable
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _services;
        private readonly ServerStartupOptions _options;
        private readonly CancellationTokenSource _tokenSource;
        private readonly EditorServerState _serverState;

        private NamedPipeServerStream? _editorServerPipe = default;
        private OmniSharpLanguageServer? _editorServer = default;

        public EditorServerApp(ILogger<EditorServerApp> logger, ServerStartupOptions options, CancellationTokenSource tokenSource, IServiceProvider services)
        {
            _logger = logger;
            _options = options;
            _tokenSource = tokenSource;
            _serverState = services.GetRequiredService<EditorServerState>();
            _services = services;
        }

        public async Task StartupAsync()
        {
            _editorServer = await OmniSharpLanguageServer.From(ConfigureServer, _services, _tokenSource.Token);
        }

        public OmniSharpLanguageServer EditorServer => _editorServer ?? throw new InvalidOperationException();
        public EditorServerState ServerState => _serverState;

        private void ConfigureServer(LanguageServerOptions options)
        {
            ConfigureTransport(options);

            var assembly = GetType().Assembly.GetName()!;
            var name = assembly.Name!;
            var version = assembly.Version!.ToString(3);

            var info = new ServerInfo
            {
                Name = name,
                Version = version
            };

            var loggerProvider = new NLogLoggerProvider();
            loggerProvider.LogFactory.Setup().LoadConfigurationFromFile("NLog-editor.config");
            options.LoggerFactory = new NLogLoggerFactory(loggerProvider);

            options
                .WithServerInfo(info)
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

                    _logger.LogDebug("Handled Initialized notification.");
                    return Task.CompletedTask;
                })

                .OnStarted(async (ILanguageServer server, CancellationToken token) =>
                {
                    _logger.LogDebug("Editor server started.");
                    token.ThrowIfCancellationRequested();

                    var folders = await server.RequestWorkspaceFolders(new(), token);
                    _serverState.WorkspaceFolders = folders ?? Container.From(Array.Empty<WorkspaceFolder>());

                    _logger.LogDebug("Handled Started notification.");
                })
                .WithHandler<ShutdownHandler>()
                .WithHandler<ExitHandler>()
                .WithHandler<DidChangeConfigurationHandler>()
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
                    ConfigurePipeIO(options);
                    break;

                default:
                    _logger?.LogWarning("An unsupported transport type was specified.");
                    throw new UnsupportedTransportTypeException(_options.TransportType);
            }
        }

        private void ConfigureStdIO(LanguageServerOptions options)
        {
            _logger.LogInformation($"Configuring editor server transport to use standard input/output streams...");

            options.WithInput(Console.OpenStandardInput());
            options.WithOutput(Console.OpenStandardOutput());
        }

        private void ConfigurePipeIO(LanguageServerOptions options)
        {
            var ioOptions = (PipeServerStartupOptions)_options;
            _logger.LogInformation("Configuring editor server transport to use a named pipe stream (mode: {mode})...", ioOptions.Mode);

            _editorServerPipe = new NamedPipeServerStream(ioOptions.PipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.CurrentUserOnly | PipeOptions.Asynchronous);
            options.WithInput(System.IO.Pipelines.PipeReader.Create(_editorServerPipe));
            options.WithOutput(System.IO.Pipelines.PipeWriter.Create(_editorServerPipe));
        }

        public void Dispose()
        {
            _editorServerPipe?.Dispose();
            _editorServer?.Dispose();

            _editorServer = null;
            _editorServerPipe = null;
        }
    }
}
