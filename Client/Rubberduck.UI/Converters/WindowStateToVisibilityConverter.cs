using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Rubberduck.UI.Converters
{
    public class WindowStateToVisibilityConverter : IValueConverter
    {
        public Visibility FalseVisibility { get; set; } = Visibility.Collapsed;
        public WindowState VisibleState { get; set; }
        public WindowState HiddenState { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var typedValue = (WindowState)value;
            return typedValue == VisibleState ? Visibility.Visible : FalseVisibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
