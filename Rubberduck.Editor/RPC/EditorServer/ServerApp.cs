using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nerdbank.Streams;
using NLog;
using NLog.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.General;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Server;
using Rubberduck.Editor.EditorServer;
using Rubberduck.Editor.RPC.EditorServer.Handlers.Lifecycle;
using Rubberduck.Editor.RPC.EditorServer.Handlers.Workspace;
using Rubberduck.InternalApi.Common;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.ServerPlatform;
using Rubberduck.InternalApi.Settings;
using Rubberduck.LanguageServer.Model;
using Rubberduck.ServerPlatform;
using Rubberduck.SettingsProvider;
using Rubberduck.SettingsProvider.Model.LanguageServer;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO.Abstractions;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using OmniSharpLanguageServer = OmniSharp.Extensions.LanguageServer.Server.LanguageServer;

namespace Rubberduck.Editor
{
    public sealed class ServerApp : IDisposable
    {
        private readonly ServerStartupOptions _options;
        private readonly CancellationTokenSource _tokenSource;

        private NamedPipeServerStream? _pipe;

        private ILogger<ServerApp>? _logger;
        private IServiceProvider? _serviceProvider;

        private OmniSharpLanguageServer _languageServer = default!;
        private EditorServerState _serverState = default!;

        public ServerApp(ServerStartupOptions options, CancellationTokenSource tokenSource)
        {
            _options = options;
            _tokenSource = tokenSource;
        }

        /// <summary>
        /// Configures and starts the editor LSP server, and then awaits an Exit notification.
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
                _serverState = _serviceProvider.GetRequiredService<EditorServerState>();

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
                throw;
            }
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<Func<ILanguageServer>>(provider => () => _languageServer);
            services.AddSingleton<ServerStartupOptions>(provider => _options);
            services.AddSingleton<Process>(provider => Process.GetProcessById((int)_options.ClientProcessId));

            services.AddSingleton<IFileSystem, FileSystem>();

            services.AddSingleton<EditorServerState>();
            services.AddSingleton<Func<EditorServerState>>(provider => () => _serverState);
            services.AddSingleton<IServerStateWriter>(provider => provider.GetRequiredService<EditorServerState>());

            services.AddSingleton<IDefaultSettingsProvider<LanguageServerSettings>>(provider => LanguageServerSettings.Default);
            services.AddSingleton<ISettingsProvider<LanguageServerSettings>, SettingsService<LanguageServerSettings>>();

            services.AddSingleton<SupportedLanguage, VisualBasicForApplicationsLanguage>();

            services.AddSingleton<IExitHandler, ExitHandler>();
            services.AddSingleton<IHealthCheckService<LanguageServerStartupSettings>, ClientProcessHealthCheckService<LanguageServerStartupSettings>>();
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
            loggerProvider.LogFactory.Setup().LoadConfigurationFromFile("NLog-editor.config");
            options.LoggerFactory = new NLogLoggerFactory(loggerProvider);

            options
                .WithServerInfo(info)
                .OnInitialize((ILanguageServer server, InitializeParams request, CancellationToken token) =>
                {
                    _logger?.LogDebug("Received Initialize request.");
                    token.ThrowIfCancellationRequested();
                    if (TimedAction.TryRun(() =>
                    {
                        _serverState.Initialize(request);
                    }, out var elapsed, out var exception))
                    {
                        _logger?.LogPerformance(TraceLevel.Verbose, "Handling initialize...", elapsed);
                    }
                    else if (exception != null)
                    {
                        _logger?.LogError(TraceLevel.Verbose, exception);
                    }
                    _logger?.LogDebug("Completed Initialize request.");
                    return Task.CompletedTask;
                })

                .OnInitialized((ILanguageServer server, InitializeParams request, InitializeResult response, CancellationToken token) =>
                {
                    _logger?.LogDebug("Received Initialized notification.");
                    token.ThrowIfCancellationRequested();

                    _logger?.LogDebug("Handled Initialized notification.");
                    return Task.CompletedTask;
                })

                .OnStarted((ILanguageServer server, CancellationToken token) =>
                {
                    _logger?.LogDebug("Language server started.");
                    token.ThrowIfCancellationRequested();

                    // todo: ?

                    _logger?.LogDebug("Handled Started notification.");
                    return Task.CompletedTask;
                })

                 /* handlers */
                 .WithHandler<SetTraceHandler>()
                 .WithHandler<DidChangeConfigurationHandler>()
                 //.WithHandler<DidChangeWatchedFileHandler>()

                 /* registrations */

                 .WithHandler<ShutdownHandler>()
                 .WithHandler<ExitHandler>()
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
        }
    }
}