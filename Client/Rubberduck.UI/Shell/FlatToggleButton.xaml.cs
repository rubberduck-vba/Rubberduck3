using System;
using System.Windows;
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
            DataContextChanged += OnDataContextChanged;
            Checked += OnCheckedChanged;
            Unchecked += OnCheckedChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            OnCheckedChanged(sender, new());
        }

        private void OnCheckedChanged(object sender, RoutedEventArgs e)
        {
            IconSource = IsChecked == true ? CheckedIcon : Icon;
        }

        public static readonly DependencyProperty HighlightBrushProperty =
            DependencyProperty.Register(nameof(HighlightBrush), typeof(Brush), typeof(FlatToggleButton));

        public Brush HighlightBrush
        {
            get => (Brush)GetValue(HighlightBrushProperty);
            set => SetValue(HighlightBrushProperty, value);
        }

        public static readonly DependencyProperty HoverBrushProperty =
            DependencyProperty.Register(nameof(HoverBrush), typeof(Brush), typeof(FlatToggleButton));

        public Brush HoverBrush
        {
            get => (Brush)GetValue(HoverBrushProperty);
            set => SetValue(HoverBrushProperty, value);
        }

        public ImageSource? IconSource 
        {
            get => (ImageSource?)GetValue(IconSourceProperty);
            set
            {
                SetProperty(IconSourceProperty, value);
            }
        }
        public static readonly DependencyProperty IconSourceProperty =
            DependencyProperty.Register(nameof(IconSource), typeof(ImageSource), typeof(FlatToggleButton));

        public ImageSource? Icon 
        {
            get => (ImageSource?)GetValue(IconProperty);
            set
            {
                SetProperty(IconProperty, value);
                SetProperty(IconSourceProperty, value);
            }
        }
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(nameof(Icon), typeof(ImageSource), typeof(FlatToggleButton));

        public ImageSource? CheckedIcon 
        {
            get => (ImageSource?)GetValue(CheckedIconProperty);
            set => SetProperty(CheckedIconProperty, value);
        }
        public static readonly DependencyProperty CheckedIconProperty =
            DependencyProperty.Register(nameof(CheckedIcon), typeof(ImageSource), typeof(FlatToggleButton));

        public static readonly DependencyProperty OffsetXProperty =
            DependencyProperty.Register(nameof(OffsetX), typeof(double), typeof(FlatToggleButton));

        public double OffsetX
        {
            get => (double)GetValue(OffsetXProperty);
            set => SetProperty(OffsetXProperty, value);
        }

        public static readonly DependencyProperty OffsetYProperty =
            DependencyProperty.Register(nameof(OffsetY), typeof(double), typeof(FlatToggleButton));

        public double OffsetY
        {
            get => (double)GetValue(OffsetYProperty);
            set => SetProperty(OffsetYProperty, value);
        }

        private void SetProperty<TValue>(DependencyProperty property, TValue value)
        {
            var oldValue = (TValue)GetValue(property);
            var newValue = value;
            if (newValue is object && !newValue.Equals(oldValue))
            {
                SetValue(property, newValue);
                OnPropertyChanged(new(property, oldValue, newValue));
            }
        }
    }
}
