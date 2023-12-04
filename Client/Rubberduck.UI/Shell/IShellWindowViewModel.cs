using System.Collections.Generic;
using Rubberduck.UI.Chrome;
using Rubberduck.UI.Shell.Document;
using Rubberduck.UI.Shell.StatusBar;

namespace Rubberduck.UI.Shell
{
    public interface IShellWindowViewModel
    {
        string Title { get; }
        IShellStatusBarViewModel StatusBar { get; }
        IEnumerable<IDocumentTabViewModel> Documents { get; }
        IWindowChromeViewModel Chrome { get; }
    }
}
