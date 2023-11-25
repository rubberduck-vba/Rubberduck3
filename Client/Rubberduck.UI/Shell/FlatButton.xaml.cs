using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Rubberduck.UI.Shell
{
    /// <summary>
    /// Interaction logic for FlatButton.xaml
    /// </summary>
    public partial class FlatButton : Button
    {
        public FlatButton()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(nameof(Icon), typeof(ImageSource), typeof(FlatButton));

        public ImageSource Icon
        {
            get => (ImageSource)GetValue(IconProperty);
            set
            {
                var oldValue = Icon;
                if (value != oldValue)
                {
                    SetValue(IconProperty, value);
                    OnPropertyChanged(new(IconProperty, oldValue, value));
                }
            }
        }
    }
}
