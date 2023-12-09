using Dragablz;
using Rubberduck.UI.Windows;

namespace Rubberduck.UI.Shell
{
    public interface IShellChildWindow
    {
        object DataContext { get; set; }
        TabablzControl Tabs { get; }
    }
}
