using Dragablz;
using Rubberduck.UI.Windows;
using System.Windows;

namespace Rubberduck.UI.Shell
{
    public partial class ShellChildToolWindow : Window, IShellChildWindow
    {
        public ShellChildToolWindow(IDragablzWindowViewModel vm) : this()
        {
            DataContext = vm;
        }

        public TabablzControl Tabs => ToolTabs;

        public ShellChildToolWindow()
        {
            InitializeComponent();
        }
    }
}
