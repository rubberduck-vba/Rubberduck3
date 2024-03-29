using Rubberduck.UI.Services.Abstract;
using System.Windows.Controls.Primitives;

namespace Rubberduck.UI.Shell.Document
{
    /// <summary>
    /// Interaction logic for TextMarkerToolTip.xaml
    /// </summary>
    public partial class TextMarkerToolTip : Popup
    {
        public TextMarkerToolTip()
        {
            InitializeComponent();
        }

        private void OnDismiss(object sender, System.Windows.RoutedEventArgs e)
        {
            IsOpen = false;
        }
    }
}
