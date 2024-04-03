using Rubberduck.UI.Windows;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Rubberduck.UI.Converters
{
    public class HasItemsVisibilityConverter : IValueConverter
    {
        public bool HiddenWhenGreaterThan { get; set; } = false;
        public int VisibleWhenGreaterThan { get; set; } = 0;
        public Visibility FalseVisibility { get; set; } = Visibility.Collapsed;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return FalseVisibility;
            }

            var count = 0;

            if (value is IEnumerable<IToolWindowViewModel> toolTabs)
            {
                count = toolTabs.Count(e => e.IsPinned);
            }
            else if (value is string s)
            {
                count = s.Length;
            }
            else if (value is IEnumerable<object> enumerable)
            {
                count = enumerable.Count();
            }
            else if (value is int n)
            {
                count = n;
            }
            else if (value is object)
            {
                count = 1;
            }

            return count > VisibleWhenGreaterThan
                ? HiddenWhenGreaterThan ? FalseVisibility : Visibility.Visible
                : HiddenWhenGreaterThan ? Visibility.Visible : FalseVisibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class HasItemsBoolConverter : IValueConverter
    {
        public bool InverseValue { get; set; }
        public int? TrueWhenExactly { get; set; }
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

            var result = TrueWhenExactly.HasValue 
                ? count == TrueWhenExactly.Value 
                : (count > TrueWhenGreaterThan);

            return InverseValue 
                ? !result 
                : result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
