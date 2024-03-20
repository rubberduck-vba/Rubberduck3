using Rubberduck.UI.Shared.Settings.Abstract;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Rubberduck.UI.Converters
{
    public class WrapPanelItemWidthMultiConverter : IMultiValueConverter
    {
        public double Margin { get; set; } = 10;
        public double SmallItemWidth { get; set; } = 380;
        public double LargeItemWidth { get; set; } = 720;

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var fullPanelWidth = (double)values[0] - Margin;
            var items = (IEnumerable<ISettingViewModel>)((ListCollectionView)values[1]).SourceCollection;
            if (values.Length > 2)
            {
                return fullPanelWidth;
            }

            var nonGroupingItems = items.Except(items.OfType<ISettingGroupViewModel>()).ToArray();
            var isSingleItem = nonGroupingItems.Length == 1;

            if (isSingleItem || fullPanelWidth <= SmallItemWidth)
            {
                return fullPanelWidth;
            }

            if (nonGroupingItems.Length >= 4 && fullPanelWidth >= 4 * SmallItemWidth)
            {
                return fullPanelWidth / 4d;
            }

            if (nonGroupingItems.Length >= 3 && fullPanelWidth >= 3 * SmallItemWidth)
            {
                return fullPanelWidth / 3d;
            }

            if (nonGroupingItems.Length >= 2 && fullPanelWidth >= 2 * SmallItemWidth)
            {
                return fullPanelWidth / 2d;
            }

            if (nonGroupingItems.Length >= 3 && nonGroupingItems.Length % 3 == 0)
            {
                var idealWidth = (int)(fullPanelWidth / 3d);
                if (idealWidth < SmallItemWidth)
                {
                    return fullPanelWidth;
                }
                return idealWidth;
            }

            if (nonGroupingItems.Length >= 2 && nonGroupingItems.Length % 2 == 0)
            {
                var idealWidth = (int)(fullPanelWidth / 2d);
                if (idealWidth < SmallItemWidth)
                {
                    return fullPanelWidth;
                }
                return idealWidth;
            }

            return fullPanelWidth;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class WrapPanelItemWidthConverter : IValueConverter
    {
        public double MinItemWidth { get; set; } = 380;
        public double MaxItemWidth { get; set; } = 720;

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
                return panelWidth / (int)maxWidthColumns - 32;
            }

            var minWidthColumns = panelWidth / MinItemWidth;
            if (minWidthColumns > 2)
            {
                return panelWidth / (int)minWidthColumns - 32;
            }


            return panelWidth;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
