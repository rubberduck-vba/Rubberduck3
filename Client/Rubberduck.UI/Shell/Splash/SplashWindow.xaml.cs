using System.Windows;

namespace Rubberduck.UI.Shell.Splash
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
            Height = 380;
            Width = 340;
            InvalidateMeasure();
        }
    }
}
