using Dragablz;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.General;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using Rubberduck.Editor.Commands;
using Rubberduck.Editor.RPC.EditorServer;
using Rubberduck.Editor.RPC.EditorServer.Handlers.Lifecycle;
using Rubberduck.Editor.RPC.EditorServer.Handlers.Workspace;
using Rubberduck.Editor.RPC.LanguageServerClient;
using Rubberduck.Editor.RPC.LanguageServerClient.Handlers;
using Rubberduck.Editor.Shell;
using Rubberduck.Editor.Shell.Document;
using Rubberduck.Editor.Shell.Splash;
using Rubberduck.Editor.Shell.StatusBar;
using Rubberduck.Editor.Shell.Tools.ServerTrace;
using Rubberduck.Editor.Shell.Tools.WorkspaceExplorer;
using Rubberduck.InternalApi.Common;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.ServerPlatform;
using Rubberduck.InternalApi.Settings;
using Rubberduck.LanguageServer.Model;
using Rubberduck.ServerPlatform;
using Rubberduck.SettingsProvider;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.SettingsProvider.Model.Editor;
using Rubberduck.SettingsProvider.Model.Editor.Tools;
using Rubberduck.SettingsProvider.Model.General;
using Rubberduck.SettingsProvider.Model.LanguageClient;
using Rubberduck.SettingsProvider.Model.LanguageServer;
using Rubberduck.SettingsProvider.Model.TelemetryServer;
using Rubberduck.SettingsProvider.Model.UpdateServer;
using Rubberduck.UI.Chrome;
using Rubberduck.UI.Command;
using Rubberduck.UI.Command.SharedHandlers;
using Rubberduck.UI.Services;
using Rubberduck.UI.Services.Abstract;
using Rubberduck.UI.Services.NewProject;
using Rubberduck.UI.Services.Settings;
using Rubberduck.UI.Shared.Message;
using Rubberduck.UI.Shared.NewProject;
using Rubberduck.UI.Shared.Settings;
using Rubberduck.UI.Shell;
using Rubberduck.UI.Shell.Document;
using Rubberduck.UI.Shell.Splash;
using Rubberduck.UI.Shell.StatusBar;
using Rubberduck.UI.Shell.Tools.ServerTrace;
using Rubberduck.UI.Shell.Tools.WorkspaceExplorer;
using Rubberduck.UI.Windows;
using Rubberduck.Unmanaged.UIContext;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO.Abstractions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

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
        private IDisposable _serverTask;

        private IUiContextProvider UIContextProvider { get; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "We want to crash the process in case of an exception anyway.")]
        protected override async void OnStartup(StartupEventArgs e)
        {
            _ = UiContextProvider.Instance(); // we MUST do this while we KNOW we're on the main thread.
            try
            {
                ShutdownMode = ShutdownMode.OnLastWindowClose;

                var args = e.Args;
                _options = await ServerArgs.ParseAsync(args);
                _tokenSource = new CancellationTokenSource();

                var services = new ServiceCollection();
                services.AddLogging(ConfigureLogging);

                ConfigureServices(services);

                services.AddSingleton<IUiDispatcher, UiDispatcher>();
                services.AddSingleton<IUiContextProvider>(provider => UIContextProvider);

                _serviceProvider = services.BuildServiceProvider();
                _logger = _serviceProvider.GetRequiredService<ILogger<App>>();

                _languageClient = new(_serviceProvider.GetRequiredService<ILogger<LanguageClientApp>>(), _options, _tokenSource, _serviceProvider);
                _settings = _serviceProvider.GetRequiredService<RubberduckSettingsProvider>();

                var splash = _serviceProvider.GetRequiredService<SplashService>();
                splash.Show(WindowSize.Splash);

                splash.UpdateStatus("Loading configuration...");
                _settings.ClearCache();

                if (_options.ClientProcessId > 0)
                {
                    _editorServer = new(_options, _tokenSource);

                    splash.UpdateStatus("Initializing language server protocol (addin/editor)...");
                    _serverTask = _editorServer.RunAsync();
                }
                else if (_settings.Settings.LanguageClientSettings.RequireAddInHost)
                {
                    throw new InvalidOperationException("Editor is not configured for standalone execution.");
                }

                if (!string.IsNullOrWhiteSpace(_options.WorkspaceRoot))
                {
                    splash.UpdateStatus("Initializing language server protocol (editor/server)...");
                    await _languageClient.StartupAsync(_settings.Settings.LanguageServerSettings.StartupSettings, new Uri(_options.WorkspaceRoot));
                }

                splash.UpdateStatus("Quack!");

                await ShowEditorAsync();
                splash.Close();
            }
            catch (Exception exception)
            {
                _logger?.LogError(exception, "An exception was thrown; editor process will exit with an error code.");
                Shutdown(1);
                throw;
            }
        }

        private async Task ShowEditorAsync()
        {
            var settings = _settings.Settings.EditorSettings;
            var model = _serviceProvider.GetRequiredService<IShellWindowViewModel>();

            var view = _shell ??= new ShellWindow() { DataContext = model };
            view.Show();

            ShowStartupToolwindows(settings);

            if (settings.ShowWelcomeTab)
            {
                await LoadWelcomeTabAsync(model);
            }
        }

        private readonly static Dictionary<Type, Type> ShowToolWindowCommandMappings = new()
        {
            [typeof(WorkspaceExplorerSettings)] = typeof(ShowWorkspaceExplorerCommand),
        };

        private void ShowStartupToolwindows(EditorSettings settings)
        {
            foreach (var toolwindow in settings.ToolsSettings.StartupToolWindows)
            {
                if (toolwindow.ShowOnStartup)
                {
                    var toolwindowSettingType = toolwindow.GetType();
                    if (ShowToolWindowCommandMappings.TryGetValue(toolwindowSettingType, out var commandType))
                    {
                        var command = (ICommand)_serviceProvider.GetRequiredService(commandType);
                        command.Execute(null);
                    }
                    else
                    {
                        _logger.LogDebug(_settings.TraceLevel, $"Could not find a command to open toolwindow from setting type '{toolwindowSettingType.Name}'.");
                    }
                }
            }
        }

        private async Task LoadWelcomeTabAsync(IShellWindowViewModel model)
        {
            var fileSystem = _serviceProvider.GetRequiredService<IFileSystem>();
            var path = fileSystem.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Rubberduck", "Templates", "Welcome.md");
            var content = await fileSystem.File.ReadAllTextAsync(path);

            var showSettingsCommand = _serviceProvider.GetRequiredService<ShowRubberduckSettingsCommand>();
            var closeToolWindowCommand = _serviceProvider.GetRequiredService<CloseToolWindowCommand>();
            var activeDocumentStatus = _serviceProvider.GetRequiredService<IDocumentStatusViewModel>();
            var welcome = new MarkdownDocumentTabViewModel(new Uri(path), "Welcome", content, isReadOnly: true, showSettingsCommand, closeToolWindowCommand, activeDocumentStatus);

            var welcomeTabContent = new MarkdownEditorControl() { DataContext = welcome };
            welcome.ContentControl = welcomeTabContent;
            welcome.IsSelected = true;

            model.DocumentWindows.Add(welcome);
            model.ActiveDocumentTab = welcome;
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
            services.AddSingleton<Func<ILanguageServer>>(provider => () => _editorServer.Server);
            services.AddSingleton<Func<ILanguageClient?>>(provider => () => _languageClient.LanguageClient);
            services.AddSingleton<LanguageClientApp>(provider => _languageClient);
            services.AddSingleton<ILanguageClientService, LanguageClientService>();
            services.AddSingleton<ILanguageServerConnectionStatusProvider, LanguageClientService>(provider => (LanguageClientService)provider.GetRequiredService<ILanguageClientService>());

            services.AddSingleton<ServerStartupOptions>(provider => _options);
            services.AddSingleton<Process>(provider => Process.GetProcessById((int)_options.ClientProcessId));

            services.AddSingleton<IFileSystem, FileSystem>();
            services.AddSingleton<PerformanceRecordAggregator>();

            services.AddSingleton<UIServiceHelper>();
            services.AddSingleton<ServerPlatformServiceHelper>();
            services.AddSingleton<EditorServerState>();
            services.AddSingleton<Func<EditorServerState>>(provider => () => (EditorServerState)_editorServer.ServerState);
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
            services.AddSingleton<INewProjectDialogService, NewProjectDialogService>();
            services.AddSingleton<IWindowFactory<NewProjectWindow, NewProjectWindowViewModel>, NewProjectWindowFactory>();
            services.AddSingleton<IVBProjectInfoProvider>(provider => null!);
            services.AddSingleton<IWorkspaceSyncService>(provider => null!);
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

            services.AddSingleton<IDocumentStatusViewModel, ActiveDocumentStatusViewModel>();
        }

        public void Dispose()
        {
            _editorServer?.Dispose();
            _languageClient?.Dispose();
            _tokenSource.Dispose();
            _serverTask.Dispose();
        }
    }
}
