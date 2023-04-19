using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Rubberduck.UI.Xaml.Controls
{
    internal class EditorShellDesignViewModel
    {
        public ObservableCollection<CodePaneDesignViewModel> ModuleDocumentTabs { get; set; } = new ObservableCollection<CodePaneDesignViewModel>
        {
            new CodePaneDesignViewModel { ModuleInfo = new ModuleInfoDesignViewModel { Name = "Module1" }},
            new CodePaneDesignViewModel { ModuleInfo = new ModuleInfoDesignViewModel { Name = "Module2" }}
        };

        public CodePaneDesignViewModel SelectedModuleDocumentTab { get; set; } = new CodePaneDesignViewModel();

        public IEnumerable<ShellToolTabDesignViewModel> ToolTabs { get; set; } = new ObservableCollection<ShellToolTabDesignViewModel>
        {
            new ShellToolTabDesignViewModel { Name = "Tooltab 1"},
            new ShellToolTabDesignViewModel { Name = "Tooltab 2"}
        };

        public ShellToolTabDesignViewModel SelectedToolTab { get; set; } = new ShellToolTabDesignViewModel();

        public StatusBarDesignViewModel Status { get; set; } = new StatusBarDesignViewModel();

    }

    internal class CodePaneDesignViewModel
    {
        public ModuleInfoDesignViewModel ModuleInfo { get; set; } = new ModuleInfoDesignViewModel();
    }

    internal class ModuleInfoDesignViewModel
    {
        public object QualifiedModuleName { get; set; } = new object();
        public string Name { get; set; } = "Module1";
    }

    internal class ShellToolTabDesignViewModel
    {
        public string Name { get; set; } = "ToolTab1";
        public object ViewModel { get; set; } = new object();
        public ShellToolTabSettingDesignViewModel Settings { get; set; } = new ShellToolTabSettingDesignViewModel();
    }

    internal class ShellToolTabSettingDesignViewModel
    {
        public bool IsVisible { get; set; } = true;
    }
}