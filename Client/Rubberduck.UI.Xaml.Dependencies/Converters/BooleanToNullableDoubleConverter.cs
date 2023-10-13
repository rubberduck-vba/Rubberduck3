using System;
using System.Globalization;
using System.Windows.Data;

namespace Rubberduck.UI.Xaml.Dependencies.Converters
{
    public class BooleanToNullableDoubleConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not double size || parameter is not IConvertible input)
            {
                return false;
            }

            try
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator - these are hard coded values.
                return System.Convert.ToDouble(input) == size;
            }
            catch
            {
                return false;
            }
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool toggle || !toggle || parameter is not IConvertible output)
            {
                return null;
            }

            try
            {
                return System.Convert.ToDouble(output);
            }
            catch
            {
                return null;
            }
        }
    }
}
