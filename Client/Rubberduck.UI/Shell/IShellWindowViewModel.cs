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
        ObservableCollection<IDocumentTabViewModel> Documents { get; }
        ObservableCollection<IToolWindowViewModel> ToolWindows { get; }

        IWindowChromeViewModel Chrome { get; }

        IInterTabClient DocumentsInterTabClient { get; }
        IInterTabClient ToolWindowInterTabClient { get; }
    }
}
