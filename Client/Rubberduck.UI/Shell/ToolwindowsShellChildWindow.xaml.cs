using Dragablz;
using Rubberduck.UI.Windows;
using System.Windows;

namespace Rubberduck.UI.Shell
{
    public partial class ToolwindowsShellChildWindow : Window, IShellChildWindow
    {
        public ToolwindowsShellChildWindow(IDragablzWindowViewModel vm) : this()
        {
            DataContext = vm;
        }

        public TabablzControl Tabs => Tabz;

        public IDragablzWindowViewModel ViewModel => (IDragablzWindowViewModel)DataContext;

        public ToolwindowsShellChildWindow()
        {
            InitializeComponent();
        }
    }
}
