using System.Windows;

namespace Rubberduck.UI.Splash
{
    /// <summary>
    /// Interaction logic for Splash.xaml
    /// </summary>
    public partial class SplashWindow : System.Windows.Window
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
