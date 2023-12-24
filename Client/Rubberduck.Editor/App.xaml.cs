using Dragablz;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.General;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using Rubberduck.Editor.Commands;
using Rubberduck.Editor.DialogServices.NewProject;
using Rubberduck.Editor.RPC.EditorServer;
using Rubberduck.Editor.RPC.EditorServer.Handlers.Lifecycle;
using Rubberduck.Editor.RPC.EditorServer.Handlers.Workspace;
using Rubberduck.Editor.RPC.LanguageServerClient.Handlers;
using Rubberduck.Editor.Shell;
using Rubberduck.Editor.Shell.Chrome;
using Rubberduck.Editor.Shell.Document.Tabs;
using Rubberduck.Editor.Shell.StatusBar;
using Rubberduck.Editor.Shell.Tools.WorkspaceExplorer;
using Rubberduck.Editor.Splash;
using Rubberduck.InternalApi.Common;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Settings;
using Rubberduck.LanguageServer.Model;
using Rubberduck.ServerPlatform;
using Rubberduck.SettingsProvider;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.SettingsProvider.Model.General;
using Rubberduck.SettingsProvider.Model.LanguageClient;
using Rubberduck.SettingsProvider.Model.LanguageServer;
using Rubberduck.SettingsProvider.Model.TelemetryServer;
using Rubberduck.SettingsProvider.Model.UpdateServer;
using Rubberduck.UI.Chrome;
using Rubberduck.UI.Command;
using Rubberduck.UI.Command.SharedHandlers;
using Rubberduck.UI.LanguageServerTrace;
using Rubberduck.UI.Message;
using Rubberduck.UI.Services;
using Rubberduck.UI.Services.Abstract;
using Rubberduck.UI.Services.Settings;
using Rubberduck.UI.Settings;
using Rubberduck.UI.Shell;
using Rubberduck.UI.Shell.Document;
using Rubberduck.UI.Shell.StatusBar;
using Rubberduck.UI.Splash;
using Rubberduck.UI.Windows;
using Rubberduck.UI.WorkspaceExplorer;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO.Abstractions;
using System.Reflection;
using System.Threading;
using System.Windows;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Rubberduck.Editor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IServiceProvider _serviceProvider;
        private ILogger<App> _logger;

        private ShellWindow _shell;

        private ServerStartupOptions _options;
        private CancellationTokenSource _tokenSource;

        private EditorServerApp _editorServer;
        private LanguageClientApp _languageClient;

        private RubberduckSettingsProvider _settings;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "We want to crash the process in case of an exception anyway.")]
        protected override async void OnStartup(StartupEventArgs e)
        {
            try
            {
                ShutdownMode = ShutdownMode.OnLastWindowClose;

                var args = e.Args;
                _options = await ServerArgs.ParseAsync(args);
                _tokenSource = new CancellationTokenSource();

                var services = new ServiceCollection();
                services.AddLogging(ConfigureLogging);

                ConfigureServices(services);

                _serviceProvider = services.BuildServiceProvider();
                _logger = _serviceProvider.GetRequiredService<ILogger<App>>();

                _languageClient = new(_serviceProvider.GetRequiredService<ILogger<LanguageClientApp>>(), _options, _tokenSource, _serviceProvider);
                _settings = _serviceProvider.GetRequiredService<RubberduckSettingsProvider>();

                var splash = _serviceProvider.GetRequiredService<SplashService>();
                splash.Show();

                splash.UpdateStatus("Loading configuration...");
                _settings.ClearCache();

                if (_options.ClientProcessId > 0)
                {
                    _editorServer = new(_serviceProvider.GetRequiredService<ILogger<EditorServerApp>>(), _options, _tokenSource, _serviceProvider);

                    splash.UpdateStatus("Initializing language server protocol (addin/editor)...");
                    await _editorServer.StartupAsync();
                }
                else if (_settings.Settings.LanguageClientSettings.RequireAddInHost)
                {
                    throw new InvalidOperationException("Editor is not configured for standalone execution.");
                }

                splash.UpdateStatus("Initializing language server protocol (editor/server)...");
                await _languageClient.StartupAsync();

                splash.UpdateStatus("Quack!");
                ShowEditor();
                splash.Close();
            }
            catch (Exception exception)
            {
                _logger?.LogError(exception, "An exception was thrown; editor process will exit with an error code.");
                Shutdown(1);
                throw;
            }
        }

        private void ShowEditor()
        {
            var model = _serviceProvider.GetRequiredService<IShellWindowViewModel>();

            // TODO "show welcome tab" setting
            var fileSystem = _serviceProvider.GetRequiredService<IFileSystem>();
            var path = fileSystem.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Rubberduck", "Templates", "Welcome.md");
            var content = fileSystem.File.ReadAllText(path);
            var welcome = new MarkdownDocumentTabViewModel(new Uri(path), "Welcome", content);

            var view = _shell ??= new ShellWindow() { DataContext = model };
            
            var welcomeTabContent = new MarkdownEditorControl() { DataContext = welcome };

            model.Documents.Add(welcome);
            view.AddDocument(welcomeTabContent);

            // prompt for new workspace here if there's no addin host?

            view.Show();

            if (_settings.Settings.EditorSettings.ToolsSettings.WorkspaceExplorerSettings.ShowOnStartup)
            {
                _serviceProvider.GetRequiredService<ShowWorkspaceExplorerCommand>().Execute(null);
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            var level = _settings.Settings.LoggerSettings.TraceLevel.ToTraceLevel();
            var delay = _settings.Settings.LanguageClientSettings.ExitNotificationDelay;

            if (TimedAction.TryRun(() =>
            {
                _languageClient?.ExitAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                base.OnExit(e);
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
            services.AddSingleton<Func<ILanguageServer>>(provider => () => _editorServer.EditorServer);
            services.AddSingleton<Func<ILanguageClient>>(provider => () => _languageClient.LanguageClient);
            services.AddSingleton<ServerStartupOptions>(provider => _options);
            services.AddSingleton<Process>(provider => Process.GetProcessById((int)_options.ClientProcessId));

            services.AddSingleton<IFileSystem, FileSystem>();
            services.AddSingleton<PerformanceRecordAggregator>();

            services.AddSingleton<UIServiceHelper>();
            services.AddSingleton<ServerPlatformServiceHelper>();
            services.AddSingleton<EditorServerState>();
            services.AddSingleton<Func<EditorServerState>>(provider => () => _editorServer.ServerState);
            services.AddSingleton<IServerStateWriter>(provider => provider.GetRequiredService<EditorServerState>());

            services.AddSingleton<IDefaultSettingsProvider<RubberduckSettings>>(provider => RubberduckSettings.Default);
            services.AddSingleton<IDefaultSettingsProvider<GeneralSettings>>(provider => GeneralSettings.Default);
            services.AddSingleton<IDefaultSettingsProvider<LanguageServerSettings>>(provider => LanguageServerSettings.Default);
            services.AddSingleton<IDefaultSettingsProvider<UpdateServerSettings>>(provider => UpdateServerSettings.Default);
            services.AddSingleton<IDefaultSettingsProvider<TelemetryServerSettings>>(provider => TelemetryServerSettings.Default);

            services.AddSingleton<ShowRubberduckSettingsCommand>();
            services.AddSingleton<RubberduckSettingsProvider>();
            services.AddSingleton<SupportedLanguage, VisualBasicForApplicationsLanguage>();

            services.AddSingleton<IExitHandler, ExitHandler>();

            services.AddSingleton<Func<LanguageClientStartupSettings>>(provider => () => provider.GetRequiredService<RubberduckSettingsProvider>().Settings.LanguageClientSettings.StartupSettings);
            services.AddSingleton<Func<LanguageServerStartupSettings>>(provider => () => provider.GetRequiredService<RubberduckSettingsProvider>().Settings.LanguageServerSettings.StartupSettings);
            services.AddSingleton<IHealthCheckService<LanguageServerStartupSettings>, ClientProcessHealthCheckService<LanguageServerStartupSettings>>();
            services.AddSingleton<IHealthCheckService<LanguageClientStartupSettings>, ClientProcessHealthCheckService<LanguageClientStartupSettings>>();

            services.AddSingleton<IWorkDoneProgressStateService, WorkDoneProgressStateService>();

            services.AddSingleton<Version>(provider => Assembly.GetExecutingAssembly().GetName().Version!);
            services.AddSingleton<SplashService>();
            services.AddSingleton<ISplashViewModel, SplashViewModel>();

            services.AddSingleton<Func<ShellWindow>>(provider => () => _shell);
            services.AddSingleton<ShellProvider>();
            services.AddSingleton<IShellWindowViewModel, ShellWindowViewModel>();
            services.AddSingleton<IShellStatusBarViewModel, ShellStatusBarViewModel>();
            services.AddSingleton<IWindowChromeViewModel, WindowChromeViewModel>();

            services.AddSingleton<InterTabClient>();
            services.AddSingleton<InterToolTabClient>();

            services.AddSingleton<ISettingsChangedHandler<RubberduckSettings>>(provider => provider.GetRequiredService<RubberduckSettingsProvider>());
            services.AddSingleton<DidChangeConfigurationHandler>();

            services.AddSingleton<MessageActionsProvider>();
            services.AddSingleton<IMessageWindowFactory, MessageWindowFactory>();
            services.AddSingleton<IMessageService, MessageService>();
            services.AddSingleton<ShowMessageHandler>();
            services.AddSingleton<ShowMessageRequestHandler>();

            services.AddSingleton<ITemplatesService, TemplatesService>();
            services.AddSingleton<IWorkspaceFolderService, WorkspaceFolderService>();
            services.AddSingleton<IProjectFileService, ProjectFileService>();

            services.AddSingleton<ISettingsDialogService, SettingsDialogService>();
            services.AddSingleton<IWindowFactory<SettingsWindow, SettingsWindowViewModel>, SettingsWindowFactory>();
            services.AddSingleton<ISettingViewModelFactory, SettingViewModelFactory>();

            services.AddSingleton<IWorkspaceService, WorkspaceService>();
            services.AddSingleton<LanguageServerTraceViewModel>();
            services.AddSingleton<IWorkspaceExplorerViewModel, WorkspaceExplorerViewModel>();
            services.AddSingleton<IWorkspaceStateManager, WorkspaceStateManager>();
            services.AddSingleton<OpenDocumentCommand>();

            services.AddSingleton<FileCommandHandlers>();
            services.AddSingleton<NewProjectCommand>();
            services.AddSingleton<NewProjectWindowFactory>();
            services.AddSingleton<OpenProjectCommand>();
            services.AddSingleton<SaveDocumentCommand>();
            services.AddSingleton<SaveDocumentAsCommand>();
            services.AddSingleton<SaveAllDocumentsCommand>();
            services.AddSingleton<SaveAsProjectTemplateCommand>();
            services.AddSingleton<CloseDocumentCommand>();
            services.AddSingleton<CloseAllDocumentsCommand>();
            services.AddSingleton<CloseWorkspaceCommand>();
            services.AddSingleton<SynchronizeWorkspaceCommand>();
            services.AddSingleton<ExitCommand>();

            services.AddSingleton<ViewCommandHandlers>();
            services.AddSingleton<ShowWorkspaceExplorerCommand>();
            services.AddSingleton<ShowLanguageServerTraceCommand>();

            services.AddSingleton<ToolsCommandHandlers>();
            services.AddSingleton<CloseToolWindowCommand>();
            services.AddSingleton<OpenLogFileCommand>();
            services.AddSingleton<ShowRubberduckSettingsCommand>();
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
        private readonly ISettingsProvider<RubberduckSettings> _settings;

        public WorkspaceRootResolver(ILogger logger, ISettingsProvider<RubberduckSettings> settings)
        {
            _logger = logger;
            _settings = settings;
        }

        protected TraceLevel TraceLevel => _settings.Settings.LoggerSettings.TraceLevel.ToTraceLevel();

        public Uri GetWorkspaceRootUri(ServerStartupOptions options)
        {
            var settings = _settings.Settings.LanguageClientSettings;
            var setting = settings.WorkspaceSettings.GetSetting<DefaultWorkspaceRootSetting>();
            var uri = setting.DefaultValue;

            var argsRoot = options.WorkspaceRoot;

            if (argsRoot is null && options.ClientProcessId == default && settings.RequireAddInHost)
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
                _logger.LogWarning("Could not parse value '{argsRoot}' into a valid URI. Falling back to default workspace root.", argsRoot);
            }

            if (settings.WorkspaceSettings.RequireDefaultWorkspaceRootHost && !uri.LocalPath.StartsWith(setting.DefaultValue.LocalPath))
            {
                _logger.LogWarning(TraceLevel, $"Configuration requires a workspace root under the default folder, but a folder under a different root was supplied.", uri.LocalPath);
                throw new NotSupportedException($"Configuration requires a workspace root under the default folder, but a folder under a different root was supplied.");
            }

            if (!settings.WorkspaceSettings.EnableUncWorkspaces && uri.IsUnc)
            {
                _logger.LogWarning(TraceLevel, $"UNC URI is not allowed: {nameof(settings.WorkspaceSettings.EnableUncWorkspaces)} setting is disabled. Default setting value will be used instead.", uri.ToString());
                uri = setting.DefaultValue;
            }

            return uri;
        }
    }
}
