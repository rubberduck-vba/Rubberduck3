using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Media;
using Icons = Rubberduck.Resources.v3.LogLevelIcons;

namespace Rubberduck.UI.Converters;

public class DiagnosticSeverityToIconConverter : ImageSourceConverter
{
    private static readonly IDictionary<DiagnosticSeverity, ImageSource> IconMap = new Dictionary<DiagnosticSeverity, ImageSource>
    {
        [DiagnosticSeverity.Error] = ToImageSource(Icons.cross_circle),
        [DiagnosticSeverity.Warning] = ToImageSource(Icons.exclamation_diamond),
        [DiagnosticSeverity.Information] = ToImageSource(Icons.information),
        [DiagnosticSeverity.Hint] = ToImageSource(Icons.information_white),
    };

    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is null)
        {
            return null!;
        }

        var level = (DiagnosticSeverity)value;
        if (IconMap.TryGetValue(level, out var icon))
        {
            return icon;
        }

        return null!;
    }
}
