using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nerdbank.Streams;
using NLog;
using NLog.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.General;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using OmniSharp.Extensions.LanguageServer.Server;
using Rubberduck.Editor.RPC;
using Rubberduck.Editor.RPC.EditorServer;
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
using System.IO.Pipelines;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using OmniSharpLanguageClient = OmniSharp.Extensions.LanguageServer.Client.LanguageClient;
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

        public OmniSharpLanguageServer LanguageServer => _editorServer ?? throw new InvalidOperationException();
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
            _logger.LogInformation($"Configuring language server transport to use standard input/output streams...");

            options.WithInput(Console.OpenStandardInput());
            options.WithOutput(Console.OpenStandardOutput());
        }

        private void ConfigurePipeIO(LanguageServerOptions options)
        {
            var ioOptions = (PipeServerStartupOptions)_options;
            _logger.LogInformation("Configuring language server transport to use a named pipe stream (mode: {mode})...", ioOptions.Mode);

            _editorServerPipe = new NamedPipeServerStream(ioOptions.PipeName, PipeDirection.InOut, 1, ioOptions.Mode, System.IO.Pipes.PipeOptions.Asynchronous);
            options.WithInput(_editorServerPipe.UsePipeReader());
            options.WithOutput(_editorServerPipe.UsePipeWriter());
        }

        public void Dispose()
        {
            _editorServerPipe?.Dispose();
            _editorServer?.Dispose();

            _editorServer = null;
            _editorServerPipe = null;
        }
    }

    /// <summary>
    /// Configures LSP for editor <--> language server communications.
    /// </summary>
    public sealed class LanguageClientApp : IDisposable
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _services;
        private readonly ServerStartupOptions _options;
        private readonly CancellationTokenSource _tokenSource;


        private Process? _languageServerProcess = default;
        private NamedPipeClientStream? _languageClientPipe = default;
        private OmniSharpLanguageClient? _languageClient = default;
        private IDisposable? _languageClientInitializeTask = default;

        public LanguageClientApp(ILogger<LanguageClientApp> logger, ServerStartupOptions options, CancellationTokenSource tokenSource, IServiceProvider services)
        {
            _logger = logger;
            _options = options;
            _services = services;
            _tokenSource = tokenSource;
        }

        public async Task StartupAsync()
        {
            StartLanguageServerProcess();

            _languageClient = await OmniSharpLanguageClient.From(ConfigureClient, _services, _tokenSource.Token);
            _languageClientInitializeTask = _languageClient.Initialize(_tokenSource.Token);
        }

        public async Task ExitAsync()
        {
            _languageClient?.SendShutdown(new());
            await SendExitServerNotificationAsync();
        }

        private async Task SendExitServerNotificationAsync()
        {
            var delay = _services.GetRequiredService<IRubberduckSettingsProvider>().Settings.LanguageClientSettings.ExitNotificationDelay;
            await Task.Delay(delay).ContinueWith(o => _languageClient?.SendExit(new()), _tokenSource.Token, TaskContinuationOptions.None, TaskScheduler.Default).ConfigureAwait(false);
        }

        private void StartLanguageServerProcess()
        {
            var settings = _services
                .GetRequiredService<ISettingsProvider<RubberduckSettings>>().Settings
                .LanguageServerSettings.StartupSettings;
            var clientProcessId = Environment.ProcessId;

            _languageServerProcess = new LanguageServerProcess(_logger).Start(clientProcessId, settings);
        }

        private void ConfigureClient(LanguageClientOptions options)
        {
            var provider = _services;
            var settings = provider.GetRequiredService<IRubberduckSettingsProvider>();

            var resolver = new WorkspaceRootResolver(_logger, settings);
            var workspaceRoot = resolver.GetWorkspaceRootUri(_options);

            ConfigureClient(options, settings.Settings, workspaceRoot);
        }

        private void ConfigureClient(LanguageClientOptions options, RubberduckSettings settings, Uri workspaceRoot)
        {
            switch (_options.TransportType)
            {
                case TransportType.StdIO:
                    ConfigureStdIO(options, settings, workspaceRoot);
                    break;

                case TransportType.Pipe:
                    ConfigurePipeIO(options, settings, workspaceRoot);
                    break;

                default:
                    _logger?.LogWarning("An unsupported transport type was specified.");
                    throw new UnsupportedTransportTypeException(_options.TransportType);
            }
        }

        private void ConfigureStdIO(LanguageClientOptions options, RubberduckSettings settings, Uri workspaceRoot)
        {
            _logger.LogInformation($"Configuring language client transport to use standard input/output streams...");
            var serverProcess = _languageServerProcess ?? throw new InvalidOperationException("BUG: Server process is not initialized.");

            options.WithInput(serverProcess.StandardOutput.BaseStream);
            options.WithOutput(serverProcess.StandardInput.BaseStream);

            LanguageClientService.ConfigureLanguageClient(Assembly.GetExecutingAssembly(), serverProcess, Environment.ProcessId, settings, workspaceRoot.LocalPath);
        }

        private void ConfigurePipeIO(LanguageClientOptions options, RubberduckSettings settings, Uri workspaceRoot)
        {
            var ioOptions = (PipeServerStartupOptions)_options;
            var pipeName = ioOptions.PipeName ?? settings.LanguageServerSettings.StartupSettings.ServerPipeName;
            _logger.LogInformation("Configuring language client transport to use a named pipe stream (name: {pipeName} mode: {mode})...", pipeName, ioOptions.Mode);

            var pipe = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut);
            options.WithInput(pipe.UsePipeReader());
            options.WithOutput(pipe.UsePipeWriter());
            LanguageClientService.ConfigureLanguageClient(Assembly.GetExecutingAssembly(), _languageClientPipe, Environment.ProcessId, settings, workspaceRoot.LocalPath);
        }

        public void Dispose()
        {
            _languageServerProcess?.Dispose();
            _languageClientPipe?.Dispose();
            _languageClient?.Dispose();
            _languageClientInitializeTask?.Dispose();

            _languageServerProcess = null;
            _languageClientPipe = null;
            _languageClient = null;
            _languageClientInitializeTask = null;
        }
    }

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<App> _logger;
        private readonly ServerStartupOptions _options;
        private readonly CancellationTokenSource _tokenSource;

        private readonly EditorServerApp _editorServer;
        private readonly LanguageClientApp _languageClient;

        private IRubberduckSettingsProvider SettingsProvider { get; }

        public App() { }

        public App(ServerStartupOptions options, CancellationTokenSource tokenSource)
        {
            _options = options;
            _tokenSource = tokenSource;

            var services = new ServiceCollection();
            services.AddLogging(ConfigureLogging);
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
            _logger = _serviceProvider.GetRequiredService<ILogger<App>>();

            _editorServer = new(_serviceProvider.GetRequiredService<ILogger<EditorServerApp>>(), options, tokenSource, _serviceProvider);
            _languageClient = new(_serviceProvider.GetRequiredService<ILogger<LanguageClientApp>>(), options, tokenSource, _serviceProvider);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "We want to crash the process in case of an exception anyway.")]
        protected override async void OnStartup(StartupEventArgs e)
        {
            try
            {
                var splash = _serviceProvider.GetRequiredService<SplashService>();
                splash.Show();

                splash.UpdateStatus("Initializing language server protocol (addin/editor)...");
                await _editorServer.StartupAsync();

                splash.UpdateStatus("Initializing language server protocol (editor/server)...");
                await _languageClient.StartupAsync();

                splash.Close();
            }
            catch (Exception exception)
            {
                _logger?.LogError(exception, "An exception was thrown; editor process will exit with an error code.");
                throw; // unhandled async void exception should crash the process here
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            var level = SettingsProvider.Settings.GeneralSettings.TraceLevel.ToTraceLevel();
            var delay = SettingsProvider.Settings.LanguageClientSettings.ExitNotificationDelay;
            if (TimedAction.TryRun(() =>
            {
                base.OnExit(e);
                _languageClient.ExitAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            }, out var elapsed, out var exception))
            {
                _logger.LogPerformance(level, $"Notified language server to shutdown and exit (delay: {delay.TotalMilliseconds}ms).", elapsed);
            }
            else if (exception is not null)
            {
                _logger.LogError(level, exception, "Error sending shutdown/exit notifications.");
            }
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
            services.AddSingleton<Func<ILanguageServer>>(provider => () => _editorServer.LanguageServer);
            services.AddSingleton<ServerStartupOptions>(provider => _options);
            services.AddSingleton<Process>(provider => Process.GetProcessById((int)_options.ClientProcessId));

            services.AddSingleton<IFileSystem, FileSystem>();

            services.AddSingleton<EditorServerState>();
            services.AddSingleton<Func<EditorServerState>>(provider => () => _editorServer.ServerState);
            services.AddSingleton<IServerStateWriter>(provider => provider.GetRequiredService<EditorServerState>());

            services.AddSingleton<IDefaultSettingsProvider<LanguageServerSettings>>(provider => LanguageServerSettings.Default);
            services.AddSingleton<IRubberduckSettingsProvider, RubberduckSettingsService>();

            services.AddSingleton<ISettingsProvider<LanguageServerSettings>, SettingsService<LanguageServerSettings>>();

            services.AddSingleton<SupportedLanguage, VisualBasicForApplicationsLanguage>();

            services.AddSingleton<IExitHandler, ExitHandler>();
            services.AddSingleton<IHealthCheckService<LanguageServerStartupSettings>, ClientProcessHealthCheckService<LanguageServerStartupSettings>>();
            services.AddSingleton<IHealthCheckService<LanguageClientStartupSettings>, ClientProcessHealthCheckService<LanguageClientStartupSettings>>();
        }


        public void Dispose()
        {
            _editorServer?.Dispose();
            _languageClient?.Dispose();
            _tokenSource.Dispose();
        }
    }

    public class WorkspaceRootResolver
    {
        private readonly ILogger _logger;
        private readonly IRubberduckSettingsProvider _settings;

        public WorkspaceRootResolver(ILogger logger, IRubberduckSettingsProvider settings)
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
