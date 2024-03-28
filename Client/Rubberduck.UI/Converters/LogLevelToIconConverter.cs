using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Media;
using Icons = Rubberduck.Resources.v3.LogLevelIcons;

namespace Rubberduck.UI.Converters;

public class LogLevelToIconConverter : ImageSourceConverter
{
    private static readonly IDictionary<LogLevel, ImageSource> IconMap = new Dictionary<LogLevel, ImageSource>
    {
        [LogLevel.Trace] = ToImageSource(Icons.information_white),
        [LogLevel.Debug] = ToImageSource(Icons.tick_circle),
        [LogLevel.Information] = ToImageSource(Icons.information),
        [LogLevel.Warning] = ToImageSource(Icons.exclamation_diamond),
        [LogLevel.Error] = ToImageSource(Icons.cross_circle),
        [LogLevel.Critical] = ToImageSource (Icons.cross_circle),
        [LogLevel.None] = null!,
    };

    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is null)
        {
            return null!;
        }

        var level = (LogLevel)value;
        if (IconMap.TryGetValue(level, out var icon))
        {
            return icon;
        }

        return null!;
    }
}