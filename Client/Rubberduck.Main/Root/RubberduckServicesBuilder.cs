//using IndenterSettings = Rubberduck.SmartIndenter.IndenterSettings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Rubberduck.Common;
using Rubberduck.Core;
using Rubberduck.Core.Editor;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.SettingsProvider.Model.LanguageServer;
using Rubberduck.Unmanaged;
using Rubberduck.Unmanaged.Abstract;
using Rubberduck.Unmanaged.Abstract.SafeComWrappers.Office;
using Rubberduck.Unmanaged.Abstract.SafeComWrappers.VB;
using Rubberduck.Unmanaged.Abstract.SourceCodeProvider;
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

namespace Rubberduck.Root
{
    internal class RubberduckServicesBuilder
    {
        private readonly IServiceCollection _services = new ServiceCollection();

        public IServiceProvider Build() => _services.BuildServiceProvider();
        public RubberduckServicesBuilder WithAddIn(IVBE vbe, IAddIn addin)
        {
            _services.AddScoped(provider => vbe);
            _services.AddScoped(provider => addin);
            _services.AddScoped<ICommandBars>(provider => new CommandBarsNonDisposingDecorator<ICommandBars>(vbe.CommandBars));

            _services.AddLogging(ConfigureLogging);
            _services.AddScoped<ILogLevelService, LogLevelService>();
            
            _services.AddScoped<IProjectsRepository>(provider => new ProjectsRepository(vbe, provider.GetRequiredService<ILogger<ProjectsRepository>>()));
            _services.AddScoped<IProjectsProvider>(provider => provider.GetRequiredService<IProjectsRepository>());
            return this;
        }

        private void ConfigureLogging(ILoggingBuilder builder)
        {
            builder.AddNLog("NLog-client.config");
        }

        public RubberduckServicesBuilder WithApplication()
        {
            _services.AddScoped<App>();
            _services.AddScoped<ISettingsService<RubberduckSettings>, SettingsService<RubberduckSettings>>();
            return this;
        }

        public RubberduckServicesBuilder WithSettingsProviders()
        {
            _services.AddScoped<ISettingsProvider<RubberduckSettings>, SettingsService<RubberduckSettings>>();
            _services.AddScoped<ISettingsProvider<LanguageServerSettingsGroup>, SettingsService<LanguageServerSettingsGroup>>();
            _services.AddScoped<ISettingsProvider<UpdateServerSettingGroup>, SettingsService<UpdateServerSettingGroup>>();
            _services.AddScoped<ISettingsProvider<TelemetryServerSettingsGroup>, SettingsService<TelemetryServerSettingsGroup>>();

            _services.AddScoped<IDefaultSettingsProvider<RubberduckSettings>>(provider => RubberduckSettings.Default);
            _services.AddScoped<IDefaultSettingsProvider<LanguageServerSettingsGroup>>(provider => LanguageServerSettingsGroup.Default);
            _services.AddScoped<IDefaultSettingsProvider<UpdateServerSettingGroup>>(provider => UpdateServerSettingGroup.Default);
            _services.AddScoped<IDefaultSettingsProvider<TelemetryServerSettingsGroup>>(provider => TelemetryServerSettingsGroup.Default);
            return this;
        }

        public RubberduckServicesBuilder WithRubberduckMenu()
        {
            _services.AddSingleton(ConfigureRubberduckParentMenu);
            _services.AddScoped<RubberduckParentMenu>();

            _services.AddScoped<IAboutCommand, AboutCommand>();
            _services.AddScoped<AboutCommandMenuItem>();

            _services.AddScoped<IShowEditorShellCommand, ShowEditorShellCommand>();
            _services.AddScoped<ShowEditorShellCommandMenuItem>();

            return this;
        }

        private static IRubberduckMenu ConfigureRubberduckParentMenu(IServiceProvider services)
        {
            var addin = services.GetRequiredService<IAddIn>();
            var vbe = services.GetRequiredService<IVBE>();

            var location = addin.CommandBarLocations[CommandBarSite.MenuBar];
            var builder = new CommandBarMenuBuilder<RubberduckParentMenu>(location, services, MainCommandBarControls(vbe, location.ParentId))
                .WithCommandMenuItem<ShowEditorShellCommandMenuItem>()
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

        public RubberduckServicesBuilder WithRubberduckEditor()
        {
            // TODO

            return this;
        }

        public RubberduckServicesBuilder WithAssemblyInfo()
        {
            _services.AddScoped<Version>(provider => Assembly.GetExecutingAssembly().GetName().Version!);
            _services.AddScoped<IOperatingSystem, WindowsOperatingSystem>();

            return this;
        }

        public RubberduckServicesBuilder WithFileSystem(IVBE vbe)
        {
            _services.AddScoped<IFileSystem, FileSystem>();
            _services.AddScoped<ITempSourceFileHandler>(provider => vbe.TempSourceFileHandler);

            return this;
        }

        public RubberduckServicesBuilder WithNativeServices(IVBE vbe)
        {
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

        public RubberduckServicesBuilder WithCommands()
        {
            _services.AddScoped<IShowEditorShellCommand, ShowEditorShellCommand>();
            _services.AddScoped<ShowEditorShellCommandMenuItem>();

            //_services.AddScoped<IAboutCommand, AboutCommand>();
            //_services.AddScoped<AboutCommandMenuItem>();

            return this;
        }
    }
}
