using System;
using System.Globalization;
using System.Windows.Data;

namespace Rubberduck.UI.Converters
{
    public class WindowToDuckyBackgroundTranslationConverter : IValueConverter
    {
        public enum TranslationDirection { TranslationX, TranslationY }

        public double Percent { get; set; } = 0.5;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var windowSize = (double)value;
            return windowSize * Percent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
