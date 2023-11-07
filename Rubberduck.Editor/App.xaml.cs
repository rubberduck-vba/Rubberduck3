using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nerdbank.Streams;
using NLog;
using NLog.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.General;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Server;
using Rubberduck.Editor.EditorServer;
using Rubberduck.Editor.RPC;
using Rubberduck.Editor.RPC.EditorServer.Handlers.Lifecycle;
using Rubberduck.Editor.RPC.EditorServer.Handlers.Workspace;
using Rubberduck.Editor.RPC.LanguageServerClient;
using Rubberduck.Editor.Splash;
using Rubberduck.InternalApi.Common;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.ServerPlatform;
using Rubberduck.InternalApi.Settings;
using Rubberduck.LanguageServer.Model;
using Rubberduck.ServerPlatform;
using Rubberduck.SettingsProvider;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.SettingsProvider.Model.LanguageClient;
using Rubberduck.SettingsProvider.Model.LanguageServer;
using Rubberduck.SettingsProvider.Model.ServerStartup;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Abstractions;
using System.IO.Pipes;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using OmniSharpLanguageClient = OmniSharp.Extensions.LanguageServer.Client.LanguageClient;
using OmniSharpLanguageServer = OmniSharp.Extensions.LanguageServer.Server.LanguageServer;

namespace Rubberduck.Editor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ServerApp> _logger;
        private readonly ServerStartupOptions _options;
        private readonly CancellationTokenSource _tokenSource;

        // LSP addin server
        private NamedPipeServerStream? _editorServerPipe = default;
        private OmniSharpLanguageServer? _editorServer = default;
        private EditorServerState? _serverState = default;

        // LSP language client
        private Process? _languageServerProcess = default;
        private NamedPipeClientStream? _languageClientPipe = default;
        private OmniSharpLanguageClient? _languageClient = default;
        private IDisposable? _languageClientInitializeTask = default;

        public App(ServerStartupOptions options, CancellationTokenSource tokenSource)
        {
            _options = options;
            _tokenSource = tokenSource;

            var services = new ServiceCollection();
            services.AddLogging(ConfigureLogging);
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
            _logger = _serviceProvider.GetRequiredService<ILogger<ServerApp>>();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            try
            {
                _serverState = _serviceProvider.GetRequiredService<EditorServerState>();

                var service = _serviceProvider.GetRequiredService<SplashService>();
                service.Show();

                service.UpdateStatus("Initializing language server protocol (addin/editor)...");
                _editorServer = await OmniSharpLanguageServer.From(ConfigureServer, _serviceProvider, _tokenSource.Token);

                service.UpdateStatus("Initializing language server protocol (editor/server)...");
                StartLanguageServerProcess(_serviceProvider);
                
                _languageClient = await OmniSharpLanguageClient.From(ConfigureClient, _serviceProvider, _tokenSource.Token);
                _languageClientInitializeTask = _languageClient.Initialize(_tokenSource.Token);
            }
            catch (Exception exception)
            {
                _logger?.LogError(exception, "An exception was thrown; editor process will exit with an error code.");
                throw; // unhandled async void exception should crash the process here
            }
        }

        private void StartLanguageServerProcess(IServiceProvider serviceProvider)
        {
            var settings = serviceProvider
                .GetRequiredService<ISettingsProvider<RubberduckSettings>>().Settings
                .LanguageServerSettings.StartupSettings;
            var clientProcessId = Environment.ProcessId;

            var logger = serviceProvider.GetRequiredService<ILogger<App>>();
            _languageServerProcess = new LanguageServerProcess(logger).Start(clientProcessId, settings);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
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
                    builder.LoadConfigurationFromFile("NLog-editor.config");
                });

                return factory;
            });
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<Func<ILanguageServer>>(provider => () => _editorServer ?? throw new InvalidOperationException("Invalid state: EditorServer is null."));
            services.AddSingleton<ServerStartupOptions>(provider => _options);
            services.AddSingleton<Process>(provider => Process.GetProcessById((int)_options.ClientProcessId));

            services.AddSingleton<IFileSystem, FileSystem>();

            services.AddSingleton<EditorServerState>();
            services.AddSingleton<Func<EditorServerState>>(provider => () => _serverState ?? throw new InvalidOperationException("Invalid state: EditorServerState is null."));
            services.AddSingleton<IServerStateWriter>(provider => provider.GetRequiredService<EditorServerState>());

            services.AddSingleton<IDefaultSettingsProvider<LanguageServerSettings>>(provider => LanguageServerSettings.Default);
            services.AddSingleton<ISettingsProvider<LanguageServerSettings>, SettingsService<LanguageServerSettings>>();

            services.AddSingleton<SupportedLanguage, VisualBasicForApplicationsLanguage>();

            services.AddSingleton<IExitHandler, ExitHandler>();
            services.AddSingleton<IHealthCheckService<LanguageServerStartupSettings>, ClientProcessHealthCheckService<LanguageServerStartupSettings>>();
        }

        private void ConfigureClient(LanguageClientOptions options)
        {
            var provider = _serviceProvider ?? throw new InvalidOperationException("BUG: IServiceProvider is not configured!");
            var settings = provider.GetRequiredService<IRubberduckSettingsProvider>();

            var resolver = new WorkspaceRootResolver(_logger, settings);
            var workspaceRoot = resolver.GetWorkspaceRootUri(_options);

            // with a host addin, workspace root folder location may come from LSP init handshake.
            // without a host addin, workspace root folder location would have to be the default as per settings.
            // if the default root is required as per settings, any init-supplied workspace folder is ignored.

            LanguageClientService.ConfigureLanguageClient(Assembly.GetExecutingAssembly(), _languageClientPipe, Environment.ProcessId, settings.Settings, workspaceRoot.LocalPath);
        }


        /// <summary>
        /// Rubberduck.Editor is a LSP server for the VBE addin, which acts as a LSP client.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
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

                    // TODO: request workspace synchronization to addin client

                    _logger?.LogDebug("Handled Started notification.");
                    return Task.CompletedTask;
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

            _editorServerPipe = new NamedPipeServerStream(ioOptions.PipeName, PipeDirection.InOut, 1, ioOptions.Mode, PipeOptions.Asynchronous);
            options.WithInput(_editorServerPipe.UsePipeReader());
            options.WithOutput(_editorServerPipe.UsePipeWriter());
        }

        private void ConfigureTransport(LanguageClientOptions options)
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

        private void ConfigureStdIO(LanguageClientOptions options)
        {
            _logger?.LogInformation($"Configuring language client transport to use standard input/output streams...");

            options.WithInput(Console.OpenStandardInput());
            options.WithOutput(Console.OpenStandardOutput());
        }

        private void ConfigurePipe(LanguageClientOptions options)
        {
            var ioOptions = (PipeServerStartupOptions)_options;
            _logger?.LogInformation("Configuring language client transport to use a named pipe stream (mode: {mode})...", ioOptions.Mode);

            _languageClientPipe = new NamedPipeClientStream(".", ioOptions.PipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
            options.WithInput(_languageClientPipe.UsePipeReader());
            options.WithOutput(_languageClientPipe.UsePipeWriter());
        }

        public void Dispose()
        {
            if (_languageClientPipe is not null)
            {
                _logger?.LogDebug("Disposing NamedPipeClientStream...");
                _languageClientPipe.Dispose();
                _languageClientPipe = null;
            }

            if (_languageClientInitializeTask is not null)
            {
                _logger?.LogDebug("Disposing LanguageClient task...");
                _languageClientInitializeTask.Dispose();
                _languageClientInitializeTask = null;
            }

            if (_editorServerPipe is not null)
            {
                _logger?.LogDebug("Disposing NamedPipeServerStream...");
                _editorServerPipe.Dispose();
                _editorServerPipe = null;
            }

            if (_editorServer is not null)
            {
                _logger?.LogDebug("Disposing EditorServer...");
                _editorServer.Dispose();
                _editorServer = null!;
            }
        }
    }

    public class WorkspaceRootResolver
    {
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        private readonly IRubberduckSettingsProvider _settings;

        public WorkspaceRootResolver(Microsoft.Extensions.Logging.ILogger logger, IRubberduckSettingsProvider settings)
        {
            _logger = logger;
            _settings = settings;
        }

        protected TraceLevel TraceLevel => _settings.Settings.GeneralSettings.TraceLevel.ToTraceLevel();

        public Uri GetWorkspaceRootUri(ServerStartupOptions options)
        {
            var settings = _settings.Settings.LanguageClientSettings;
            var setting = settings.GetSetting<DefaultWorkspaceRootSetting>();
            var uri = setting.DefaultValue;

            var argsRoot = options.WorkspaceRoot;

            if (argsRoot is null && settings.RequireAddInHost)
            {
                _logger.LogDebug("An add-in host is required and should have provided a workspace root command-line argument. The current configuration does not support standalone execution.");
                throw new NotSupportedException("An add-in host is required and should have provided a workspace root command-line argument. The current configuration does not support standalone execution.");
            }
            else if (argsRoot is null)
            {
                // editor is running standalone without an addin client connection.
                _logger.LogDebug("No workspace root commad-line argument was specified, but configuration supports standalone execution. Using default workspace root; there is no project file or workspace folder yet.");
                return setting.DefaultValue;
            }

            if (Uri.TryCreate(argsRoot, UriKind.Absolute, out var argsRootUri))
            {
                uri = argsRootUri;
            }
            else
            {
                _logger.LogWarning($"Could not parse value '{argsRoot}' into a valid URI. Falling back to default workspace root.");
            }

            if (settings.RequireDefaultWorkspaceRootHost && !uri.LocalPath.StartsWith(setting.DefaultValue.LocalPath))
            {
                _logger.LogWarning(TraceLevel, $"Configuration requires a workspace root under the default folder, but a folder under a different root was supplied.", uri.LocalPath);
                throw new NotSupportedException($"Configuration requires a workspace root under the default folder, but a folder under a different root was supplied.");
            }

            if (!settings.EnableUncWorkspaces && uri.IsUnc)
            {
                _logger.LogWarning(TraceLevel, $"UNC URI is not allowed: {nameof(settings.EnableUncWorkspaces)} setting is disabled. Default setting value will be used instead.", uri.ToString());
                uri = setting.DefaultValue;
            }

            return uri;
        }
    }
}
