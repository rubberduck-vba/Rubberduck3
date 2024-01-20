using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Rubberduck.UI.Shell.Tools.ServerTrace
{
    public class LogLevelFilterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var level = (LogLevel)value;
            if (parameter is LogMessageFiltersViewModel vm)
            {
                return vm.Filters.Contains(level)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }

            throw new ArgumentException("Invalid parameter or value", nameof(value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null!;
        }
    }
}
