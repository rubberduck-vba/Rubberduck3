using System.Windows;
using Rubberduck.UI.Xaml.Shell;

namespace Rubberduck.UI.Xaml
{
    /// <summary>
    /// Interaction logic for ShellWindow.xaml
    /// </summary>
    public partial class ShellWindow : Window
    {
        public ShellWindow(ShellWindowViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }

        public ShellWindow()
        {
            InitializeComponent();
        }

        public ShellWindowViewModel ViewModel => (ShellWindowViewModel)DataContext;
    }
}
