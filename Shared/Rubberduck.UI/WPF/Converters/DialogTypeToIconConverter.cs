using Rubberduck.InternalApi.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Media;
using Icons = Rubberduck.Resources.DialogTypeIcons;

namespace Rubberduck.UI.WPF.Converters
{
    public enum DialogType
    {
        None,
        Information,
        Warning,
        Error,
        Question
    }

    public class DialogTypeToIconConverter : ImageSourceConverter
    {
        private static readonly IDictionary<DialogType, ImageSource> DialogTypeIcons = new Dictionary<DialogType, ImageSource>
        {
            [DialogType.None] = ToImageSource(Icons.cross_circle),
            [DialogType.Information] = ToImageSource(Icons.information),
            [DialogType.Error] = ToImageSource(Icons.cross_circle),
            [DialogType.Question] = ToImageSource(Icons.information_white),
        };

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return null;
            }

            var moduleType = (DialogType)value;
            if (DialogTypeIcons.TryGetValue(moduleType, out var icon))
            {
                return icon;
            }

            return null;
        }
    }
}