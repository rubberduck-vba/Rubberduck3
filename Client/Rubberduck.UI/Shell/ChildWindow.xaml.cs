using Dragablz;
using Rubberduck.UI.Windows;
using System.Windows;

namespace Rubberduck.UI.Shell
{
    /// <summary>
    /// Interaction logic for ChildWindow.xaml
    /// </summary>
    public partial class ChildWindow : Window
    {
        public ChildWindow(IDragablzWindowViewModel vm) : this()
        {
            DataContext = vm;
        }

        public TabablzControl Tabs => Tabz;

        public ChildWindow()
        {
            InitializeComponent();
        }
    }
}
