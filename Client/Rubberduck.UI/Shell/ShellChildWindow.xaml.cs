using Dragablz;
using Rubberduck.UI.Windows;
using System.Windows;

namespace Rubberduck.UI.Shell
{
    public partial class ShellChildWindow : Window, IShellChildWindow
    {
        public ShellChildWindow(IDragablzWindowViewModel vm) : this()
        {
            DataContext = vm;
        }

        public TabablzControl Tabs => Tabz;

        public ShellChildWindow()
        {
            InitializeComponent();
        }
    }
}
