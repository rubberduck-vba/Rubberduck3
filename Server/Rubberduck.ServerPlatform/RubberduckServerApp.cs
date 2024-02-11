using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
//using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.WorkDone;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using OmniSharp.Extensions.LanguageServer.Server;
using Rubberduck.InternalApi.Common;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Settings;
using Rubberduck.InternalApi.Settings.Model;
using Rubberduck.InternalApi.Settings.Model.ServerStartup;
using Rubberduck.ServerPlatform.Logging;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO.Pipelines;
using System.IO.Pipes;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using OmniSharpLanguageServer = OmniSharp.Extensions.LanguageServer.Server.LanguageServer;

namespace Rubberduck.ServerPlatform
{
    public class LanguageServerProvider
    {
        internal static OmniSharpLanguageServer? Server { get; set; }

        public ILanguageServer? LanguageServer => Server;
    }

    public abstract class RubberduckServerApp<TSettings, TStartupSettings> : IDisposable
        where TSettings : RubberduckSetting
        where TStartupSettings : RubberduckSetting, IHealthCheckSettingsProvider
    {
        private readonly ServerStartupOptions _startupOptions;
        private readonly CancellationTokenSource _tokenSource;
        private readonly bool _requireWorkspaceUri;

        private IServiceProvider? _serviceProvider;
        private ILogger? _logger;
        private IDisposable? _pipeWaitForClientConnectionTask;
        private NamedPipeServerStream? _pipe;

        private OmniSharpLanguageServer _server = default!;
        private ServerState<TSettings, TStartupSettings> _serverState = default!;

        protected RubberduckServerApp(ServerStartupOptions options, CancellationTokenSource tokenSource, bool requireWorkspaceUri = false)
        {
            _startupOptions = options;
            _tokenSource = tokenSource;
            _requireWorkspaceUri = requireWorkspaceUri;
        }

        public async Task RunAsync()
        {
            try
            {
                var services = new ServiceCollection();
                services.AddLogging(ConfigureLogging);

                ConfigureServices(_startupOptions, services);

                _serviceProvider = services.BuildServiceProvider();
                _logger = _serviceProvider.GetRequiredService<ILogger<RubberduckServerApp<TSettings, TStartupSettings>>>();
                _serverState = GetServerState(_serviceProvider);

                _server = await OmniSharpLanguageServer.From(ConfigureServer, _serviceProvider, _tokenSource.Token);
                LanguageServerProvider.Server = _server;

                await _server.WaitForExit;
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

        public ServerState<TSettings, TStartupSettings> ServerState => _serverState;
        public OmniSharpLanguageServer Server => _server;

        protected abstract ServerState<TSettings, TStartupSettings> GetServerState(IServiceProvider provider);
        protected abstract void ConfigureServices(ServerStartupOptions options, IServiceCollection services);
        protected virtual void ValidateWorkspaceRoot(string requestUri)
        {
            // no-op unless overridden
        }


        protected virtual void ConfigureLogging(ILoggingBuilder builder)
        {
            LogManager.Setup(setup =>
            {
                setup.SetupExtensions(ext =>
                {
                    ext.RegisterTarget<LanguageServerClientLoggerTarget>();
                });
            });

            builder.Services.AddSingleton<LanguageServerProvider>();

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

            ConfigureHandlers(options
                .WithServerInfo(info)
                .OnInitialize(OnLanguageServerInitializeAsync)
                .OnInitialized(OnLanguageServerInitializedAsync)
                .OnStarted(OnLanguageServerStartedAsync));
        }

        protected abstract void ConfigureHandlers(LanguageServerOptions options);

        private async Task OnLanguageServerInitializeAsync(ILanguageServer server, InitializeParams request, CancellationToken token)
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
                        onError: (exception) => new WorkDoneProgressEnd
                        {
                            Message = request.Trace == InitializeTrace.Verbose ? exception.ToString() : exception.Message
                        },
                        onComplete: () => new WorkDoneProgressEnd
                        {
                            Message = "Completed."
                        });
                try
                {
                    progress?.OnNext("Processing initialization parameters...", null, null);

                    if (request.ProcessId != _startupOptions.ClientProcessId)
                    {
                        throw new InvalidOperationException($"Request ProcessId={request.ProcessId} mismatched expected ClientProcessId={_startupOptions.ClientProcessId}.");
                    }
                    _serverState.Initialize(request);

                    if (_requireWorkspaceUri)
                    {
                        progress?.OnNext("Validating workspace root...", null, null);
                        ValidateWorkspaceRoot(request.RootUri?.GetFileSystemPath() ?? request.RootPath ?? throw new InvalidOperationException("Workspace root URI was not supplied."));

                    }
                    //var workspaceRoot = new System.IO.DirectoryInfo(_serverState.RootUri?.GetFileSystemPath() ?? request.RootPath ?? throw new InvalidOperationException("Workspace root URI was not supplied."));
                    //if (!workspaceRoot.Exists)
                    //{
                    //    throw new InvalidOperationException("Specified workspace does not exist.");
                    //}
                    //_logger?.LogTrace(TraceLevel.Verbose, "Validated workspace root.", workspaceRoot.FullName);

                    progress?.OnCompleted();
                }
                catch (Exception exception)
                {
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
        }

        private async Task OnLanguageServerInitializedAsync(ILanguageServer server, InitializeParams request, InitializeResult response, CancellationToken cancellationToken)
        {
            // OnLanguageServerInitializedDelegate
            // Gives your class or handler an opportunity to interact with the InitializeParams and InitializeResult
            // after it is processed by the server but before it is sent to the client.

            _logger?.LogTrace("Sending Initialized notification. InitializeResult: {0}", JsonSerializer.Serialize(response));
        }

        private async Task OnLanguageServerStartedAsync(ILanguageServer server, CancellationToken token)
        {
            // OnLanguageServerStartedDelegate
            // Gives your class or handler an opportunity to interact with the ILanguageServer
            // after the connection has been established.

            _logger?.LogDebug("Server started.");
            token.ThrowIfCancellationRequested();

            IWorkDoneObserver? progress = null;
            try
            {
                var handlerProgressToken = new ProgressToken(Guid.NewGuid().ToString());

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
                    _logger?.LogTrace("WorkDoneManager.IsSupported returned false; client will not be sent progress notifications from this server.");
                }

                var service = server.Services.GetRequiredService<ServerPlatformServiceHelper>();
                await OnServerStartedAsync(server, token, progress, service);
            }
            catch (Exception exception)
            {
                _logger?.LogError(exception.ToString());
            }
            finally
            {
                progress?.Dispose();
            }
        }

        protected abstract Task OnServerStartedAsync(ILanguageServer server, CancellationToken token, IWorkDoneObserver? progress, ServerPlatformServiceHelper service);

        private void ConfigureTransport(LanguageServerOptions options)
        {
            switch (_startupOptions.TransportType)
            {
                case TransportType.StdIO:
                    ConfigureStdIO(options);
                    break;

                case TransportType.Pipe:
                    ConfigurePipeIO(options);
                    break;

                default:
                    _logger?.LogWarning("An unsupported transport type was specified.");
                    throw new UnsupportedTransportTypeException(_startupOptions.TransportType);
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
            var ioOptions = (PipeServerStartupOptions)_startupOptions;
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
                _pipeWaitForClientConnectionTask?.Dispose();
                _pipe.Dispose();
                _pipe = null;
            }

            if (_server is not null)
            {
                _logger?.LogDebug("Disposing LSP server...");
                _server.Dispose();
                _server = null!;
            }
        }

    }
}