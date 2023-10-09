//using IndenterSettings = Rubberduck.SmartIndenter.IndenterSettings;
using Dragablz;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Rubberduck.Common;
using Rubberduck.Common.Hotkeys;
using Rubberduck.Core;
using Rubberduck.Core.About;
using Rubberduck.Core.Editor;
using Rubberduck.Interaction.MessageBox;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.SettingsProvider;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.UI;
using Rubberduck.UI.RubberduckEditor;
using Rubberduck.UI.Splash;
using Rubberduck.UI.Xaml.Shell;
using Rubberduck.UI.Xaml.Splash;
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
            
            _services.AddScoped<ISplashViewModel, SplashViewModel>();
            _services.AddScoped<SplashService>();

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
            _services.AddScoped<ISettingsProvider<LanguageServerSettings>, SettingsService<LanguageServerSettings>>();
            _services.AddScoped<ISettingsProvider<UpdateServerSettings>, SettingsService<UpdateServerSettings>>();

            _services.AddScoped<IDefaultSettingsProvider<RubberduckSettings>>(provider => RubberduckSettings.Default);
            _services.AddScoped<IDefaultSettingsProvider<LanguageServerSettings>>(provider => LanguageServerSettings.Default);
            _services.AddScoped<IDefaultSettingsProvider<UpdateServerSettings>>(provider => UpdateServerSettings.Default);
            return this;
        }

        public RubberduckServicesBuilder WithRubberduckMenu()
        {
            _services.AddSingleton(ConfigureRubberduckParentMenu);
            _services.AddScoped<RubberduckParentMenu>();

            _services.AddScoped<IAboutCommand, AboutCommand>();
            _services.AddScoped<AboutCommandMenuItem>();

            //_services.AddScoped<IShowEditorShellCommand, ShowEditorShellCommand>();
            _services.AddScoped<ShowEditorShellCommandMenuItem>();

            return this;
        }

        private static IRubberduckMenu ConfigureRubberduckParentMenu(IServiceProvider services)
        {
            var addin = services.GetRequiredService<IAddIn>();
            var vbe = services.GetRequiredService<IVBE>();

            var location = addin.CommandBarLocations[CommandBarSite.MenuBar];
            var builder = new CommandBarMenuBuilder<RubberduckParentMenu>(location, services, MainCommandBarControls(vbe, location.ParentId))
                .WithCommandMenuItem<AboutCommandMenuItem>()
                .WithSeparator()
                .WithCommandMenuItem<ShowEditorShellCommandMenuItem>();

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
            //_services.AddScoped<IPresenter, EditorShellWindowPresenter>();
            _services.AddScoped<ShellWindowViewModel>();
            _services.AddScoped<IInterTabClient, ShellInterTabClient>();

            //_services.AddScoped<IEditorShellViewModel, EditorShellViewModel>();
            //_services.AddScoped<IStatusBarViewModel, StatusBarViewModel>();

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
            //_services.AddScoped<IBeepInterceptor>(provider => new BeepInterceptor(nativeApi));
            _services.AddScoped<IVbeEvents>(provider => VbeEvents.Initialize(vbe));
            _services.AddScoped<IVBETypeLibsAPI, VBETypeLibsAPI>();

            _services.AddScoped<IUiDispatcher, UiDispatcher>();
            _services.AddScoped<IUiContextProvider>(provider => UiContextProvider.Instance());

            _services.AddScoped<IRubberduckHooks, RubberduckHooks>();
            _services.AddScoped<HotkeyFactory>();

            return this;
        }

        public RubberduckServicesBuilder WithCommands()
        {
            _services.AddScoped<IShowEditorShellCommand, ShowEditorShellCommand>();
            _services.AddScoped<ShowEditorShellCommandMenuItem>();

            _services.AddScoped<IAboutCommand, AboutCommand>();
            _services.AddScoped<AboutCommandMenuItem>();
            _services.AddScoped<IWebNavigator, WebNavigator>();
            _services.AddScoped<IMessageBox, FormsMessageBox>(); // TODO implement a WpfMessageBox

            return this;
        }
    }
}
