using System.Windows;
using System.Windows.Input;
using Rubberduck.UI.Abstract;

namespace Rubberduck.UI.Xaml.Controls
{
    /// <summary>
    /// Interaction logic for AboutControl.xaml
    /// </summary>
    public partial class AboutControl
    {
        public AboutControl()
        {
            InitializeComponent();
        }

        public AboutControl(IAboutControlViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }

        private IAboutControlViewModel ViewModel => DataContext as IAboutControlViewModel;

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            var isControlCPressed = Keyboard.IsKeyDown(Key.C) && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl));
            if (isControlCPressed)
            {
                ViewModel?.CopyVersionInfo();
            }
        }

        private void CopyVersionInfo_Click(object sender, RoutedEventArgs e)
        {
            ViewModel?.CopyVersionInfo();
        }
    }
}
