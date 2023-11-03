using System;
using System.Globalization;
using System.Windows.Data;

namespace Rubberduck.UI.Converters
{
    public class WindowToDuckyBackgroundTranslationConverter : IValueConverter
    {
        public enum TranslationDirection { TranslationX, TranslationY }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var windowSize = System.Convert.ToDouble(value);
            var percent = System.Convert.ToDouble(parameter);
            return windowSize * percent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
