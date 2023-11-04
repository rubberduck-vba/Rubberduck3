using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Rubberduck.UI.Shell
{
    /// <summary>
    /// Interaction logic for FlatButton.xaml
    /// </summary>
    public partial class FlatToggleButton : ToggleButton
    {
        public FlatToggleButton()
        {
            InitializeComponent();
        }

        public ImageSource? Icon { get; set; }
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(nameof(Icon), typeof(ImageSource), typeof(FlatToggleButton));

        public ImageSource? PressedIcon { get; set; }
        public static readonly DependencyProperty PressedIconProperty =
            DependencyProperty.Register(nameof(PressedIcon), typeof(ImageSource), typeof(FlatToggleButton));
    }
}
