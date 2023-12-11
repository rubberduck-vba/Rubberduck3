using Rubberduck.UI.Windows;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Rubberduck.UI.Converters
{
    public class HasItemsBoolConverter : IValueConverter
    {
        public int TrueWhenGreaterThan { get; set; } = 0;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var count = 0;

            if (value is IEnumerable<IToolWindowViewModel> toolTabs)
            {
                count = toolTabs.Count(e => e.IsPinned);
            }
            else if (value is IEnumerable<object> enumerable)
            {
                count = enumerable.Count();
            }
            else if (value is int n)
            {
                count = n;
            }

            return count > TrueWhenGreaterThan;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
