using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Rubberduck.UI.Styles
{
    /// <summary>
    /// Interaction logic for ControlBoxButton.xaml
    /// </summary>
    public partial class ControlBoxCanvas : Canvas
    {
        public ImageSource? ImageSource { get; set; }
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register(nameof(ImageSource), typeof(ImageSource), typeof(ControlBoxCanvas), new PropertyMetadata((obj, args) =>
            {
                var target = (ControlBoxCanvas)obj;
                target.ImageSource = (ImageSource)args.NewValue;
            }));
    }
}
