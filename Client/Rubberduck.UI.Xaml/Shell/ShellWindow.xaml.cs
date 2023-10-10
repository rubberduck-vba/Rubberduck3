using System.Windows;
using Rubberduck.InternalApi.Model.Abstract;

namespace Rubberduck.UI.Xaml
{
    /// <summary>
    /// Interaction logic for ShellWindow.xaml
    /// </summary>
    public partial class ShellWindow : Window
    {
        public ShellWindow(IShellWindowViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }

        public ShellWindow()
        {
            InitializeComponent();
        }

        public IShellWindowViewModel ViewModel => (IShellWindowViewModel)DataContext;
    }
}
