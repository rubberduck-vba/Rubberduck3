using System.Collections.ObjectModel;
using Dragablz;
using Rubberduck.UI.Chrome;
using Rubberduck.UI.Shell.Document;
using Rubberduck.UI.Shell.StatusBar;
using Rubberduck.UI.Windows;

namespace Rubberduck.UI.Shell
{
    public interface IShellWindowViewModel
    {
        string Title { get; }
        IShellStatusBarViewModel StatusBar { get; }
        ObservableCollection<IDocumentTabViewModel> DocumentWindows { get; }
        ObservableCollection<IToolWindowViewModel> FloatingToolwindows { get; }
        ObservableCollection<IToolWindowViewModel> LeftPanelToolWindows { get; }
        ObservableCollection<IToolWindowViewModel> RightPanelToolWindows { get; }
        ObservableCollection<IToolWindowViewModel> BottomPanelToolWindows { get; }

        int FixedLeftToolTabs { get; }
        int FixedRightToolTabs { get; }
        int FixedBottomToolTabs { get; }
        int FixedDocumentTabs { get; }

        IToolPanelViewModel LeftToolPanel { get; }
        IToolPanelViewModel RightToolPanel { get; }
        IToolPanelViewModel BottomToolPanel { get; }

        IWindowChromeViewModel Chrome { get; }

        IInterTabClient InterToolTabClient { get; }
        IInterTabClient InterTabClient { get; }
        ItemActionCallback ClosingTabItemHandler { get; }

        IDocumentTabViewModel ActiveDocumentTab { get; set; }
    }
}
