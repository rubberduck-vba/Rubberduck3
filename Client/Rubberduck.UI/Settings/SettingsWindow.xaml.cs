using System.Windows;

namespace Rubberduck.UI.Settings
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow(ISettingsWindowViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }

        public SettingsWindow()
        {
            InitializeComponent();
        }
    }
}
