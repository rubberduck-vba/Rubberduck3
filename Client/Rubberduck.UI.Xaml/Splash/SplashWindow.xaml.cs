using System.Windows;

namespace Rubberduck.UI.Xaml.Splash
{
    /// <summary>
    /// Interaction logic for Splash.xaml
    /// </summary>
    public partial class SplashWindow : Window
    {
        public SplashWindow(ISplashViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }

        public SplashWindow()
        {
            InitializeComponent();
        }
    }
}
