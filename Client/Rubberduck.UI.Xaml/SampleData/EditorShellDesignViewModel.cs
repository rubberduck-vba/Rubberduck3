using Rubberduck.InternalApi.Model;
using Rubberduck.UI.Abstract;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

///<summary>
///This file provides design-time data for the <see cref="EditorShellControl.xaml"/> control.
///This is only to support working in the XAML designer and nothing in this file should be used otherwise.
///</summary>
namespace Rubberduck.UI.Xaml.Controls
{
    internal class EditorShellDesignViewModel : IEditorShellViewModel
    {
        public ObservableCollection<ICodePaneViewModel> ModuleDocumentTabs { get; set; } = new ObservableCollection<ICodePaneViewModel>
        {
            new CodePaneDesignViewModel { ModuleInfo = new ModuleInfoDesignViewModel { Name = "Module1" }},
            new CodePaneDesignViewModel { ModuleInfo = new ModuleInfoDesignViewModel { Name = "Class1" }}
        };

        public ICodePaneViewModel SelectedModuleDocumentTab
        {
            get => ModuleDocumentTabs.FirstOrDefault();
            set { }
        }

        public IEnumerable<IShellToolTab> ToolTabs { get; set; } = new ObservableCollection<IShellToolTab>
        {
            new ShellToolTabDesignViewModel { Name = "Left tooltab 1"},
            new ShellToolTabDesignViewModel { Name = "Left tooltab 2"}
//            new ShellToolTabDesignViewModel { Name = "Right tooltab 1", Settings = new ShellToolTabSettingDesignViewModel { TabPanelLocation = ToolTabLocation.RightPanel } } //Note : filtering doesn't send this to the right panel in the designer
        };

        public IShellToolTab SelectedToolTab
        {
            get => ToolTabs.FirstOrDefault();
            set { }
        }
        public IStatusBarViewModel Status { get; set; } = new StatusBarDesignViewModel();

        public IEnumerable<ISyntaxErrorViewModel> SyntaxErrors => throw new System.NotImplementedException();
        public event PropertyChangedEventHandler PropertyChanged;
        public void ActivateModuleDocumentTab(IQualifiedModuleName module) => throw new System.NotImplementedException();
        public ICodePaneViewModel GetModule(IQualifiedModuleName module) => throw new System.NotImplementedException();
        public bool LoadModule(IQualifiedModuleName module, string content, IMemberProviderViewModel vm) => throw new System.NotImplementedException();
        public bool UnloadModule(IQualifiedModuleName module) => throw new System.NotImplementedException();
    }

    internal class ShellToolTabDesignViewModel : IShellToolTab
    {
        public string Name { get; set; }
        public object ViewModel { get; set; } = new SyncPanelDesignViewModel() as ISyncPanelViewModel;
        public IShellToolTabSetting Settings { get; set; } = new ShellToolTabSettingDesignViewModel();
    }

    internal class ShellToolTabSettingDesignViewModel : IShellToolTabSetting
    {
        public bool IsVisible { get; set; } = true;
        public ToolTabLocation TabPanelLocation { get; set; } = ToolTabLocation.LeftPanel;
        public bool IsLoadedAtStartup { get; set; } = true;
    }
}