//using IndenterSettings = Rubberduck.SmartIndenter.IndenterSettings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Rubberduck.Common;
using Rubberduck.Core;
using Rubberduck.Core.About;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Main.Commands.ShowRubberduckEditor;
using Rubberduck.Main.RPC.EditorServer;
using Rubberduck.SettingsProvider;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.SettingsProvider.Model.LanguageServer;
using Rubberduck.UI.Command;
using Rubberduck.UI.Message;
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

namespace Rubberduck.Main.Root
{
    internal class RubberduckServicesBuilder
    {
        private readonly IServiceCollection _services = new ServiceCollection();

        public RubberduckServicesBuilder(IVBE vbe, IAddIn addin)
        {
            Configure(vbe, addin);
        }

        public IServiceProvider Build() => _services.BuildServiceProvider();
        private void Configure(IVBE vbe, IAddIn addin)
        {
            _services.AddLogging(ConfigureLogging);
            _services.AddScoped<App>();

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

            _services.AddScoped<ILogLevelService, LogLevelService>();
        }

        private RubberduckServicesBuilder WithAssemblyInfo()
        {
            _services.AddScoped(provider => Assembly.GetExecutingAssembly().GetName().Version!);
            _services.AddScoped<IOperatingSystem, WindowsOperatingSystem>();

            return this;
        }

        private RubberduckServicesBuilder WithNativeServices(IVBE vbe, IAddIn addin)
        {
            _services.AddScoped(provider => vbe);
            _services.AddScoped(provider => addin);
            _services.AddScoped(provider => vbe.TempSourceFileHandler);

            _services.AddScoped<IProjectsRepository>(provider => new ProjectsRepository(vbe, provider.GetRequiredService<ILogger<ProjectsRepository>>()));
            _services.AddScoped<IProjectsProvider>(provider => provider.GetRequiredService<IProjectsRepository>());

            var nativeApi = new VbeNativeApiAccessor();
            _services.AddScoped<IVbeNativeApi>(provider => nativeApi);
            _services.AddScoped<IVbeEvents>(provider => VbeEvents.Initialize(vbe));
            _services.AddScoped<IVBETypeLibsAPI, VBETypeLibsAPI>();

            #region still needed?
            _services.AddScoped<IUiDispatcher, UiDispatcher>();
            _services.AddScoped<IUiContextProvider>(provider => UiContextProvider.Instance());
            #endregion

            return this;
        }

        private RubberduckServicesBuilder WithSettingsProviders()
        {
            // ISettingsService<TSettings> provides file I/O
            _services.AddScoped<ISettingsService<RubberduckSettings>, SettingsService<RubberduckSettings>>();

            // IDefaultSettingsProvider<TSettings> provide the default configuration settings for injectable setting groups
            _services.AddScoped<IDefaultSettingsProvider<RubberduckSettings>>(provider => RubberduckSettings.Default);
            _services.AddScoped<IDefaultSettingsProvider<LanguageServerSettingsGroup>>(provider => LanguageServerSettingsGroup.Default);
            _services.AddScoped<IDefaultSettingsProvider<UpdateServerSettingsGroup>>(provider => UpdateServerSettingsGroup.Default);
            _services.AddScoped<IDefaultSettingsProvider<TelemetryServerSettingsGroup>>(provider => TelemetryServerSettingsGroup.Default);

            // ISettingsProvider<TSettings> provide current applicable settings for injectable setting groups
            _services.AddScoped<ISettingsProvider<RubberduckSettings>, SettingsService<RubberduckSettings>>();
            _services.AddScoped<ISettingsProvider<LanguageServerSettingsGroup>, SettingsService<LanguageServerSettingsGroup>>();
            _services.AddScoped<ISettingsProvider<UpdateServerSettingsGroup>, SettingsService<UpdateServerSettingsGroup>>();
            _services.AddScoped<ISettingsProvider<TelemetryServerSettingsGroup>, SettingsService<TelemetryServerSettingsGroup>>();

            return this;
        }

        private RubberduckServicesBuilder WithRubberduckMenu(IVBE vbe)
        {
            _services.AddSingleton<ICommandBars>(provider => new CommandBarsNonDisposingDecorator<ICommandBars>(vbe.CommandBars));

            _services.AddSingleton<RubberduckParentMenu>();

            _services.AddSingleton<IAboutCommand, AboutCommand>();
            _services.AddSingleton<AboutCommandMenuItem>();

            _services.AddSingleton<IShowRubberduckEditorCommand, Commands.ShowRubberduckEditor.ShowRubberduckEditorCommand>();
            _services.AddSingleton<ShowRubberduckEditorCommandMenuItem>();

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
            _services.AddScoped<IFileSystem, FileSystem>();

            _services.AddScoped<IMessageService, MessageService>();
            _services.AddScoped<IMessageWindowFactory, MessageWindowFactory>();
            _services.AddScoped<MessageActionsProvider>();

            _services.AddSingleton<IEditorServerProcessService, EditorServerProcessService>();

            return this;
        }

        private RubberduckServicesBuilder WithRubberduckEditorServer()
        {

            return this;
        }
    }
}
