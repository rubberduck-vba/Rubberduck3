using Rubberduck.UI.Xaml.Dependencies.Controls.StatusBar;
using Rubberduck.UI.Xaml.Shell.Document;
using System.Collections.Generic;

namespace Rubberduck.UI.Xaml.Shell
{
    public interface IShellWindowViewModel
    {
        string Title { get; }
        IStatusBarViewModel StatusBar { get; }
        IEnumerable<IDocumentTabViewModel> DocumentTabs { get; }
    }
}
