using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Media;
using Icons = Rubberduck.Resources.RubberduckUI;
//using Rubberduck.Parsing.Annotations.Concrete;
//using Rubberduck.Parsing.Symbols;
//using Rubberduck.Resources.CodeExplorer;

namespace Rubberduck.UI.Converters
{
    /*
    public class ModuleSyncStateToIconConverter : ImageSourceConverter
    {
        private static readonly IDictionary<ModuleSyncState, ImageSource> ModuleSyncStateIcons = new Dictionary<ModuleSyncState, ImageSource>
        {
            [ModuleSyncState.NotLoaded] = ToImageSource(Icons.document_smiley_sad),
            [ModuleSyncState.OK] = ToImageSource(Icons.tick_circle),
            [ModuleSyncState.ModifiedRDE] = ToImageSource(Icons.document_pencil),
            [ModuleSyncState.ModifiedVBE] = ToImageSource(Icons.document_exclamation),
            [ModuleSyncState.DeletedRDE] = ToImageSource(Icons.document_minus),
            [ModuleSyncState.DeletedVBE] = ToImageSource(Icons.document_broken),
        };

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return null;
            }

            var moduleSyncState = (ModuleSyncState)value;
            if (ModuleSyncStateIcons.TryGetValue(moduleSyncState, out var icon))
            {
                return icon;
            }

            return null;
        }
    }
    */
}