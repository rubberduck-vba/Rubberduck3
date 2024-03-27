using Rubberduck.Resources.v3;
using System;
using System.Globalization;
using System.Windows.Media;

namespace Rubberduck.UI.Converters
{
    public class SearchImageSourceConverter : ImageSourceConverter
    {
        private readonly ImageSource _search = ToImageSource(Icons.circle_arrow_right_solid);
        private readonly ImageSource _clear = ToImageSource(Icons.xmark_solid);

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty(value?.ToString()) ? _search : _clear;
        }
    }
}
