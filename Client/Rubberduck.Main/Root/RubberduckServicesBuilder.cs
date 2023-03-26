//using IndenterSettings = Rubberduck.SmartIndenter.IndenterSettings;
using Microsoft.Extensions.DependencyInjection;
using Rubberduck.Common;
using Rubberduck.Core.About;
using Rubberduck.Core.Editor;
using Rubberduck.Core.Editor.Tools;
using Rubberduck.Interaction.MessageBox;
using Rubberduck.InternalApi.UIContext;
using Rubberduck.Parsing.Abstract;
using Rubberduck.Parsing.TokenStreamProviders;
using Rubberduck.SettingsProvider;
using Rubberduck.UI;
using Rubberduck.UI.Abstract;
using Rubberduck.UI.Command;
using Rubberduck.UI.WinForms;
using Rubberduck.VBEditor.ComManagement;
using Rubberduck.VBEditor.ComManagement.NonDisposingDecorators;
using Rubberduck.VBEditor.ComManagement.TypeLibs;
using Rubberduck.VBEditor.ComManagement.TypeLibs.Abstract;
using Rubberduck.VBEditor.Events;
using Rubberduck.VBEditor.SafeComWrappers.Abstract;
using Rubberduck.VBEditor.SourceCodeHandling;
using Rubberduck.VBEditor.UI;
using Rubberduck.VBEditor.UI.OfficeMenus;
using Rubberduck.VBEditor.UI.OfficeMenus.RubberduckMenu;
using Rubberduck.VBEditor.VbeRuntime;
using Rubberduck.VersionCheck;
using System;
using System.IO;
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
            _services.AddSingleton<IVBE>(vbe);
            _services.AddSingleton<IAddIn>(addin);
            _services.AddSingleton<ICommandBars>(new CommandBarsNonDisposingDecorator<ICommandBars>(vbe.CommandBars));

            var repository = new ProjectsRepository(vbe);
            _services.AddSingleton<IProjectsProvider>(repository);
            //_services.AddSingleton<IProjectsRepository>(repository);

            return this;
        }

        public RubberduckServicesBuilder WithRubberduckEditor()
        {
            _services.AddSingleton<IDockablePresenter, EditorShellDockablePresenter>();
            _services.AddSingleton<IEditorShellWindowProvider, EditorShellWindowProvider>();

            _services.AddSingleton<IEditorShellViewModel, EditorShellViewModel>();
            _services.AddSingleton<IShellToolTabProvider, ShellToolTabProvider>();
            _services.AddSingleton<IStatusBarViewModel, StatusBarViewModel>();

            _services.AddSingleton<ISyncPanelToolTab, SyncPanelToolTab>();
            _services.AddSingleton<ISyncPanelViewModel, SyncPanelViewModel>();
            _services.AddSingleton<ISyncPanelModuleViewModelProvider, SyncPanelModuleViewModelProvider>();

            return this;
        }

        public RubberduckServicesBuilder WithAssemblyInfo()
        {
            _services.AddSingleton<Version>(_ => Assembly.GetExecutingAssembly().GetName().Version);

            _services.AddSingleton<IFileSystem, FileSystem>();
            _services.AddSingleton<IOperatingSystem, WindowsOperatingSystem>();

            return this;
        }

        public RubberduckServicesBuilder WithParser()
        {
            _services.AddSingleton<ICommonTokenStreamProvider<TextReader>, TextReaderTokenStreamProvider>();

            return this;
        }

        public RubberduckServicesBuilder WithFileSystem(IVBE vbe)
        { 
            _services.AddSingleton<ITempSourceFileHandler>(vbe.TempSourceFileHandler);
            _services.AddSingleton<IPersistencePathProvider>(PersistencePathProvider.Instance);

            return this;
        }

        public RubberduckServicesBuilder WithNativeServices(IVBE vbe)
        {
            var nativeApi = new VbeNativeApiAccessor();
            _services.AddSingleton<IVbeNativeApi>(nativeApi);
            _services.AddSingleton<IBeepInterceptor>(new BeepInterceptor(nativeApi));
            _services.AddSingleton<IVbeEvents>(VbeEvents.Initialize(vbe));
            _services.AddSingleton<IVBETypeLibsAPI, VBETypeLibsAPI>();
            
            _services.AddSingleton<IUiContextProvider>(UiContextProvider.Instance());

            return this;
        }

        public RubberduckServicesBuilder WithCommands()
        {
            _services.AddSingleton<IShowEditorShellCommand, ShowEditorShellCommand>();
            _services.AddSingleton<ShowEditorShellCommandMenuItem>();

            _services.AddSingleton<IAboutCommand, AboutCommand>();
            _services.AddSingleton<AboutCommandMenuItem>();
            _services.AddSingleton<IWebNavigator, WebNavigator>();
            _services.AddSingleton<IMessageBox, FormsMessageBox>(); // TODO implement a WpfMessageBox

            _services.AddSingleton<VersionCheckCommand>();
            _services.AddSingleton<IVersionCheckService, VersionCheckService>();

            return this;
        }
    }
}
