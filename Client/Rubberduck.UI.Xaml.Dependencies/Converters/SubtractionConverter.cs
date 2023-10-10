using System;
using System.Globalization;
using System.Windows.Data;

namespace Rubberduck.UI.Xaml.Dependencies.Converters
{
    public class SubtractionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var typedValue = (double)value;
            if (!double.TryParse((string)parameter, out var typedParam))
            {
                return (double)value;
            }
            return typedValue - typedParam;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var typedValue = (double)value;
            if (!double.TryParse((string)parameter, out var typedParam))
            {
                return (double)value;
            }
            return typedValue + typedParam;
        }
    }
}
