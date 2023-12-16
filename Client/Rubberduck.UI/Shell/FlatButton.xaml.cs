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

        public static readonly DependencyProperty OffsetXProperty =
            DependencyProperty.Register(nameof(OffsetX), typeof(double), typeof(FlatButton));

        public double OffsetX
        {
            get => (double)GetValue(OffsetXProperty);
            set
            {
                var oldValue = OffsetX;
                if (value != oldValue)
                {
                    SetValue(OffsetXProperty, value);
                    OnPropertyChanged(new(OffsetXProperty, oldValue, value));
                }
            }
        }

        public static readonly DependencyProperty OffsetYProperty =
            DependencyProperty.Register(nameof(OffsetY), typeof(double), typeof(FlatButton));

        public double OffsetY
        {
            get => (double)GetValue(OffsetYProperty);
            set
            {
                var oldValue = OffsetY;
                if (value != oldValue)
                {
                    SetValue(OffsetYProperty, value);
                    OnPropertyChanged(new(OffsetYProperty, oldValue, value));
                }
            }
        }
    }
}
