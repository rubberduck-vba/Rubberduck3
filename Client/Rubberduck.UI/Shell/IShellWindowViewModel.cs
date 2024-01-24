using System.Collections.ObjectModel;
using Dragablz;
using Rubberduck.InternalApi.Settings.Model.Editor.Tools;
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

    public interface IToolPanelViewModel
    {
        /// <summary>
        /// The location of the tool panel.
        /// </summary>
        DockingLocation PanelLocation { get; }

        /// <summary>
        /// Whether the tool panel is currently expanded.
        /// </summary>
        bool IsExpanded { get; set; }

        /// <summary>
        /// Whether the tool panel remains expanded on mouse leave.
        /// </summary>
        bool IsPinned { get; set; }
        bool IsDocked => PanelLocation != DockingLocation.None;
    }
}
