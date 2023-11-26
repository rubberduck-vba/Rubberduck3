using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Rubberduck.UI.Converters
{
    public class WrapPanelItemWidthConverter : IValueConverter
    {
        public double MinItemWidth { get; set; } = 300;
        public double MaxItemWidth { get; set; } = 512;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var panelWidth = (double)value;
            if (panelWidth <= MinItemWidth)
            {
                return MinItemWidth;
            }

            var maxWidthColumns = panelWidth / MaxItemWidth;
            if (maxWidthColumns > 2)
            {
                return panelWidth / (int)maxWidthColumns;
            }

            var minWidthColumns = panelWidth / MinItemWidth;
            if (minWidthColumns > 1.5)
            {
                return panelWidth / (int)minWidthColumns;
            }

            return panelWidth;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
