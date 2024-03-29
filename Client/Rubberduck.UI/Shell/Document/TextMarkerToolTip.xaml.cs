using Rubberduck.UI.Services.Abstract;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

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
            var thumb = new Thumb { Width = 0, Height = 0 };
            ContentCanvas.Children.Add(thumb);

            MouseDown += (sender, e) => thumb.RaiseEvent(e);
            thumb.DragDelta += (sender, e) =>
            {
                HorizontalOffset += e.HorizontalChange;
                VerticalOffset += e.VerticalChange;
            };
        }

        private void OnDismiss(object sender, RoutedEventArgs e)
        {
            IsOpen = false;
        }
    }
}
