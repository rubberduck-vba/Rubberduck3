using System.Windows;

namespace Rubberduck.UI.Shell.About
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow() : this(null!) { }

        public AboutWindow(IAboutWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        public IAboutWindowViewModel ViewModel => (IAboutWindowViewModel)DataContext;
    }
}
