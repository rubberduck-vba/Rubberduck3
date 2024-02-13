//using IndenterSettings = Rubberduck.SmartIndenter.IndenterSettings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Rubberduck.Environment;
using Rubberduck.InternalApi.Common;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Workspace;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.InternalApi.Settings.Model;
using Rubberduck.InternalApi.Settings.Model.General;
using Rubberduck.InternalApi.Settings.Model.LanguageServer;
using Rubberduck.InternalApi.Settings.Model.ServerStartup;
using Rubberduck.InternalApi.Settings.Model.TelemetryServer;
using Rubberduck.InternalApi.Settings.Model.UpdateServer;
using Rubberduck.Main.About;
using Rubberduck.Main.Commands.ApplicationTips;
using Rubberduck.Main.Commands.NewWorkspace;
using Rubberduck.Main.Commands.ShowRubberduckEditor;
using Rubberduck.Main.RPC.EditorServer;
using Rubberduck.Main.Settings;
using Rubberduck.Parsing._v3;
using Rubberduck.Parsing.Abstract;
using Rubberduck.Parsing.Exceptions;
using Rubberduck.Parsing.Parsers;
using Rubberduck.Parsing.PreProcessing;
using Rubberduck.Parsing.TokenStreamProviders;
using Rubberduck.ServerPlatform;
using Rubberduck.UI.Command;
using Rubberduck.UI.Command.SharedHandlers;
using Rubberduck.UI.Services;
using Rubberduck.UI.Services.Abstract;
using Rubberduck.UI.Services.NewProject;
using Rubberduck.UI.Services.Settings;
using Rubberduck.UI.Shared.About;
using Rubberduck.UI.Shared.Message;
using Rubberduck.UI.Shared.NewProject;
using Rubberduck.UI.Shared.Settings;
using Rubberduck.UI.Shell.About;
using Rubberduck.UI.Windows;
using Rubberduck.Unmanaged;
using Rubberduck.Unmanaged.Abstract;
using Rubberduck.Unmanaged.Abstract.SafeComWrappers.Office;
using Rubberduck.Unmanaged.Abstract.SafeComWrappers.VB;
using Rubberduck.Unmanaged.Events;
using Rubberduck.Unmanaged.NonDisposingDecorators;
using Rubberduck.Unmanaged.TypeLibs.Abstract;
using Rubberduck.Unmanaged.TypeLibs.Public;
using Rubberduck.Unmanaged.UIContext;
using Rubberduck.Unmanaged.VBERuntime;
using Rubberduck.VBEditor.UI.OfficeMenus;
using Rubberduck.VBEditor.UI.OfficeMenus.RubberduckMenu;
using System;
using System.Globalization;
using System.IO.Abstractions;
using System.Reflection;
using System.Threading;
using Env = System.Environment;

namespace Rubberduck.Main.Root
{
    internal class RubberduckServicesBuilder
    {
        private readonly IServiceCollection _services = new ServiceCollection();
        private readonly CancellationTokenSource _tokenSource;

        private string? _workspaceRoot = null;

        private IUiContextProvider UIContextProvider { get; }

        public RubberduckServicesBuilder(IVBE vbe, IAddIn addin, CancellationTokenSource tokenSource)
        {
            Configure(vbe, addin);
            _tokenSource = tokenSource;

            UIContextProvider = UiContextProvider.Instance();
        }

        public IServiceProvider Build() => _services.BuildServiceProvider();
        private void Configure(IVBE vbe, IAddIn addin)
        {
            _services.AddLogging(ConfigureLogging);
            _services.AddSingleton<App>();

            this.WithAssemblyInfo()
                .WithNativeServices(vbe, addin)
                .WithSettingsProviders()
                .WithRubberduckMenu(vbe)
                .WithServices()
                .WithRubberduckEditorServer();
        }

        private void ConfigureLogging(ILoggingBuilder builder)
        {
            builder.AddNLog("NLog-client.config");

            _services.AddSingleton<ILogLevelService, LogLevelService>();
            _services.AddSingleton<PerformanceRecordAggregator>();
        }

        private RubberduckServicesBuilder WithAssemblyInfo()
        {
            _services.AddSingleton<Version>(provider => Assembly.GetExecutingAssembly().GetName().Version!);
            _services.AddSingleton<IOperatingSystem, WindowsOperatingSystem>();

            return this;
        }

        private RubberduckServicesBuilder WithNativeServices(IVBE vbe, IAddIn addin)
        {
            _services.AddSingleton(provider => vbe);
            _services.AddSingleton(provider => addin);
            _services.AddSingleton(provider => vbe.TempSourceFileHandler);
            _services.AddSingleton<IRubberduckFoldersService, RubberduckFoldersService>();
            _services.AddSingleton<IProjectsRepository>(provider => new ProjectsRepository(vbe, provider.GetRequiredService<ILogger<ProjectsRepository>>()));
            _services.AddSingleton<IProjectsProvider>(provider => provider.GetRequiredService<IProjectsRepository>());

            var nativeApi = new VbeNativeApiAccessor();
            _services.AddSingleton<IVbeNativeApi>(provider => nativeApi);
            _services.AddSingleton<IVBETypeLibsAPI, VBETypeLibsAPI>();
            _services.AddSingleton<ICompilationArgumentsProvider, CompilationArgumentsProvider>();
            _services.AddSingleton<ITypeLibWrapperProvider, TypeLibWrapperProvider>();
            _services.AddSingleton<VBAPredefinedCompilationConstants>(provider => new VBAPredefinedCompilationConstants(double.Parse(vbe.Version, CultureInfo.InvariantCulture)));

            _services.AddSingleton<IUiDispatcher, UiDispatcher>();
            _services.AddSingleton<IUiContextProvider>(provider => UIContextProvider);

            return this;
        }

        private RubberduckServicesBuilder WithSettingsProviders()
        {
            // ISettingsService<TSettings> provides file I/O
            _services.AddSingleton<RubberduckSettingsProvider>();
            _services.AddSingleton<ISettingsService<RubberduckSettings>>(provider => provider.GetRequiredService<RubberduckSettingsProvider>());

            // IDefaultSettingsProvider<TSettings> provide the default configuration settings for injectable setting groups
            _services.AddSingleton<IDefaultSettingsProvider<RubberduckSettings>>(provider => RubberduckSettings.Default);
            _services.AddSingleton<IDefaultSettingsProvider<GeneralSettings>>(provider => GeneralSettings.Default);
            _services.AddSingleton<IDefaultSettingsProvider<LanguageServerSettings>>(provider => LanguageServerSettings.Default);
            _services.AddSingleton<IDefaultSettingsProvider<UpdateServerSettings>>(provider => UpdateServerSettings.Default);
            _services.AddSingleton<IDefaultSettingsProvider<TelemetryServerSettings>>(provider => TelemetryServerSettings.Default);

            // ISettingsProvider<TSettings> provide current applicable settings for injectable setting groups
            _services.AddSingleton<ISettingsProvider<RubberduckSettings>, SettingsService<RubberduckSettings>>();
            _services.AddSingleton<ISettingsProvider<GeneralSettings>, SettingsService<GeneralSettings>>();
            _services.AddSingleton<ISettingsProvider<LanguageServerSettings>, SettingsService<LanguageServerSettings>>();
            _services.AddSingleton<ISettingsProvider<UpdateServerSettings>, SettingsService<UpdateServerSettings>>();
            _services.AddSingleton<ISettingsProvider<TelemetryServerSettings>, SettingsService<TelemetryServerSettings>>();

            return this;
        }

        private RubberduckServicesBuilder WithRubberduckMenu(IVBE vbe)
        {
            _services.AddSingleton<ICommandBars>(provider => new CommandBarsNonDisposingDecorator<ICommandBars>(vbe.CommandBars));

            _services.AddSingleton<RubberduckParentMenu>();

            _services.AddSingleton<IAboutCommand, AboutCommand>();
            _services.AddSingleton<AboutCommandMenuItem>();
            _services.AddSingleton<AboutService>();
            _services.AddSingleton<IAboutWindowViewModel, AboutWindowViewModel>();

            _services.AddSingleton<IShowRubberduckEditorCommand, ShowRubberduckEditorCommand>();
            _services.AddSingleton<ShowRubberduckEditorCommandMenuItem>();
            _services.AddSingleton<EditorServerProcessService>();

            _services.AddSingleton<IShowApplicationTipsCommand, ShowApplicationTipsCommand>();
            _services.AddSingleton<ShowAplicationTipsCommandMenuItem>();
            
            _services.AddSingleton<INewWorkspaceCommand, NewWorkspaceCommand>();
            _services.AddSingleton<ITemplatesService, TemplatesService>();
            _services.AddSingleton<IWorkspaceFolderService, WorkspaceFolderService>();
            _services.AddSingleton<IProjectFileService, ProjectFileService>();
            _services.AddSingleton<NewProjectCommand>();
            _services.AddSingleton<NewWorkspaceCommandMenuItem>();
            _services.AddSingleton<IDialogService<NewProjectWindowViewModel>, NewProjectDialogService>();
            _services.AddSingleton<IWindowFactory<NewProjectWindow, NewProjectWindowViewModel>, NewProjectWindowFactory>();
            _services.AddSingleton<IVBProjectInfoProvider, ProjectInfoProvider>();
            _services.AddSingleton<IWorkspaceService>(provider => null!); // add-in doesn't open workspaces

            _services.AddSingleton<ShowRubberduckSettingsCommand>();
            _services.AddSingleton<ISettingsCommand, SettingsCommand>();
            _services.AddSingleton<SettingsCommandMenuItem>();
            
            _services.AddSingleton(ConfigureRubberduckMenu);
            return this;
        }

        private static IRubberduckMenu ConfigureRubberduckMenu(IServiceProvider services)
        {
            var addin = services.GetRequiredService<IAddIn>();
            var vbe = services.GetRequiredService<IVBE>();

            var location = addin.CommandBarLocations[CommandBarSite.MenuBar];
            var builder = new CommandBarMenuBuilder<RubberduckParentMenu>(location, services, MainCommandBarControls(vbe, location.ParentId))
                .WithCommandMenuItem<NewWorkspaceCommandMenuItem>()
                .WithCommandMenuItem<ShowRubberduckEditorCommandMenuItem>()
                .WithSeparator()
                .WithCommandMenuItem<SettingsCommandMenuItem>()
                .WithSeparator()
                //.WithCommandMenuItem<ShowAplicationTipsCommandMenuItem>()
                .WithCommandMenuItem<AboutCommandMenuItem>();

            return builder.Build();
        }

        private static ICommandBarControls MainCommandBarControls(IVBE vbe, int commandBarIndex)
        {
            ICommandBarControls controls;
            using (var commandBars = vbe.CommandBars)
            {
                using var menuBar = commandBars[commandBarIndex];
                controls = menuBar.Controls;
            }
            return controls;
        }


        private RubberduckServicesBuilder WithServices()
        {
            _services.AddSingleton<IFileSystem, FileSystem>();

            _services.AddSingleton<IMessageService, MessageService>();
            _services.AddSingleton<IMessageWindowFactory, MessageWindowFactory>();
            _services.AddSingleton<MessageActionsProvider>();
            _services.AddSingleton<CloseToolWindowCommand>();
            _services.AddSingleton<ShellProvider>(provider => null!); // we do NOT want to inject the RD3 shell into the add-in process.

            _services.AddSingleton<ISettingsDialogService, SettingsDialogService>();
            _services.AddSingleton<IWindowFactory<SettingsWindow, SettingsWindowViewModel>, SettingsWindowFactory>();
            _services.AddSingleton<ISettingViewModelFactory, SettingViewModelFactory>();

            _services.AddSingleton<UIServiceHelper>();

            // limited in-process parsing activities
            _services.AddSingleton<IWorkspaceSyncService, WorkspaceSyncService>();
            _services.AddSingleton<WorkspaceFolderMigrationService>();
            _services.AddSingleton<IParser<string>, TokenStreamParserAdapterWithPreprocessing<string>>();
            _services.AddSingleton<ICommonTokenStreamProvider<string>, StringTokenStreamProvider>();
            _services.AddSingleton<ITokenStreamParser, VBATokenStreamParser>();
            _services.AddSingleton<VBAPreprocessorParser>();
            _services.AddSingleton<ITokenStreamPreprocessor, VBAPreprocessor>();
            return this;
        }

        private RubberduckServicesBuilder WithRubberduckEditorServer()
        {
            _services.AddSingleton<ILanguageClientService, EditorClientService>();
            _services.AddSingleton<EditorClientApp>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<EditorClientApp>>();
                var settings = provider.GetRequiredService<RubberduckSettingsProvider>().Settings.LanguageClientSettings;
                
                var startup = settings.StartupSettings;

                ServerStartupOptions options = 
                startup.ServerTransportType == TransportType.Pipe
                    ? new PipeServerStartupOptions
                    {
                        ClientProcessId = Env.ProcessId,
                        WorkspaceRoot = _workspaceRoot,
                        Name = startup.ServerPipeName,
                        Mode = System.IO.Pipes.PipeTransmissionMode.Message,
                        Silent = startup.ServerTraceLevel == MessageTraceLevel.Off,
                        Verbose = startup.ServerTraceLevel == MessageTraceLevel.Verbose,                    
                    }
                    : new StandardInOutServerStartupOptions
                    {
                        ClientProcessId = Env.ProcessId,
                        WorkspaceRoot = _workspaceRoot,
                        Silent = startup.ServerTraceLevel == MessageTraceLevel.Off,
                        Verbose = startup.ServerTraceLevel == MessageTraceLevel.Verbose,
                    };
                return new EditorClientApp(logger, provider, options, _tokenSource);
            });
            _services.AddSingleton<IWorkDoneProgressStateService, WorkDoneProgressStateService>();
            return this;
        }

        private bool ConfirmCreateDefaultWorkspaceRoot(IMessageService messages, string defaultWorkspaceRoot, string hostDocumentFullFileName)
        {
            var acceptAction = new MessageAction
            {
                IsDefaultAction = true,
                ResourceKey = "AcceptAction_ConfirmCreateDefaultWorkspaceRoot",
                ToolTipKey = "ToolTip_ConfirmCreateDefaultWorkspaceRoot",
            };

            var hostFile = System.IO.Path.GetFileName(hostDocumentFullFileName);
            var workspaceName = System.IO.Path.GetFileNameWithoutExtension(hostFile);

            var model = new MessageRequestModel
            {
                Key = "Workspace_DefaultWorkspaceRootRequired",
                Title = "Default Workspace Root Required",
                Message = "The current configuration requires that the host document for the workspace/project be saved under the *default workspace root* folder. **Create a new workspace folder for this project?**",
                Verbose = $"The host document will be saved under `{defaultWorkspaceRoot}\\{workspaceName}\\{hostFile}`.", // oh yeah, how?
                MessageActions = [acceptAction, MessageAction.CancelAction],
                Level = LogLevel.Warning
            };

            return messages.ShowMessageRequest(model)?.MessageAction == acceptAction;
        }

        private bool ConfirmCreateWorkspaceFolder(IMessageService messages, string path)
        {
            var acceptAction = new MessageAction
            {
                IsDefaultAction = true,
                ResourceKey = "AcceptAction_ConfirmCreateWorkspace",
                ToolTipKey = "ToolTip_ConfirmCreateWorkspace",
            };

            var model = new MessageRequestModel
            {
                Key = "Workspace_ConfirmCreateWorkspace",
                Title = "Create Workspace",
                Message = $"This will create a new Rubberduck workspace under folder `{path}`.",
                Verbose = $"The folder will contain a `{ProjectFile.FileName}` Rubberduck project file and a `{ProjectFile.SourceRoot}` folder where the source files will be exported. Consider using the same local root folder for all Rubberduck projects/workspaces.",
                Level = LogLevel.Information,
                MessageActions = [acceptAction, MessageAction.CancelAction],
            };

            return messages.ShowMessageRequest(model)?.MessageAction == acceptAction;
        }

        private void MessageSavedHostIsRequired(IMessageService messages)
        {
            var model = new MessageModel
            {
                Key = "Workspace_SavedHostDocumentRequired",
                Title = "Unsaved Host Document",
                Message = "The current configuration requires that the host document for the workspace/project be saved to disk first.",
                Level = LogLevel.Warning
            };

            messages.ShowMessage(model, provider => provider.OkOnly());
        }

        private bool ConfirmStartWithoutWorkspace(IMessageService messages)
        {
            var acceptAction = new MessageAction
            {
                IsDefaultAction = true,
                ResourceKey = "AcceptAction_ConfirmStartWithoutWorkspace",
                ToolTipKey = "ToolTip_ConfirmStartWithoutWorkspace",
            };

            var model = new MessageRequestModel
            {
                Key = "Workspace_NoWorkspaceForUnsavedHost",
                Title = "No Workspace?",
                Message = "The host document is not saved. Start the editor without a workspace?",
                MessageActions = [acceptAction, MessageAction.CancelAction],
                Level = LogLevel.Information
            };

            return messages.ShowMessageRequest(model)?.MessageAction == acceptAction;
        }
    }
}
