using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Rubberduck.UI.Xaml.Dependencies.Converters
{
    public class NonZeroToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is int number))
            {
                return false;
            }

            return number != 0 ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
