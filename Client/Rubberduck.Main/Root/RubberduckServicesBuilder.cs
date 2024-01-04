﻿//using IndenterSettings = Rubberduck.SmartIndenter.IndenterSettings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Rubberduck.Environment;
using Rubberduck.InternalApi.Common;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Main.About;
using Rubberduck.Main.Commands.ShowRubberduckEditor;
using Rubberduck.Main.RPC.EditorServer;
using Rubberduck.Main.Settings;
using Rubberduck.ServerPlatform;
using Rubberduck.SettingsProvider;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.SettingsProvider.Model.General;
using Rubberduck.SettingsProvider.Model.LanguageServer;
using Rubberduck.SettingsProvider.Model.ServerStartup;
using Rubberduck.SettingsProvider.Model.TelemetryServer;
using Rubberduck.SettingsProvider.Model.UpdateServer;
using Rubberduck.UI.About;
using Rubberduck.UI.Command;
using Rubberduck.UI.Command.SharedHandlers;
using Rubberduck.UI.Message;
using Rubberduck.UI.Services;
using Rubberduck.UI.Services.Settings;
using Rubberduck.UI.Settings;
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

        public RubberduckServicesBuilder(IVBE vbe, IAddIn addin, CancellationTokenSource tokenSource)
        {
            Configure(vbe, addin);
            _tokenSource = tokenSource;
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
            _services.AddSingleton<IVbeEvents>(provider => VbeEvents.Initialize(vbe));
            _services.AddSingleton<IVBETypeLibsAPI, VBETypeLibsAPI>();

            #region still needed?
            _services.AddSingleton<IUiDispatcher, UiDispatcher>();
            _services.AddSingleton<IUiContextProvider>(provider => UiContextProvider.Instance());
            #endregion

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
            _services.AddSingleton<IEditorServerProcessService, EditorServerProcessService>();

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
                .WithCommandMenuItem<ShowRubberduckEditorCommandMenuItem>()
                .WithSeparator()
                .WithCommandMenuItem<SettingsCommandMenuItem>()
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

            _services.AddSingleton<IEditorServerProcessService, EditorServerProcessService>();
            _services.AddSingleton<UIServiceHelper>();
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
                        WorkspaceRoot = settings.WorkspaceSettings.DefaultWorkspaceRoot.LocalPath,
                        Name = startup.ServerPipeName,
                        Mode = System.IO.Pipes.PipeTransmissionMode.Message,
                        Silent = startup.ServerTraceLevel == MessageTraceLevel.Off,
                        Verbose = startup.ServerTraceLevel == MessageTraceLevel.Verbose,                    
                    }
                    : new StandardInOutServerStartupOptions
                    {
                        ClientProcessId = Env.ProcessId,
                        WorkspaceRoot = settings.WorkspaceSettings.DefaultWorkspaceRoot.LocalPath,
                        Silent = startup.ServerTraceLevel == MessageTraceLevel.Off,
                        Verbose = startup.ServerTraceLevel == MessageTraceLevel.Verbose,
                    };
                return new EditorClientApp(logger, provider, options, _tokenSource);
            });
            _services.AddSingleton<IWorkDoneProgressStateService, WorkDoneProgressStateService>();
            return this;
        }
    }
}
