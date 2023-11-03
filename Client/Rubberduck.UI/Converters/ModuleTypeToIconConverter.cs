using Rubberduck.InternalApi.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Media;
using Icons = Rubberduck.Resources.MemberTypeIcons;

namespace Rubberduck.UI.Converters
{
    public class ModuleTypeToIconConverter : ImageSourceConverter
    {
        private static readonly IDictionary<ModuleType, ImageSource> ModuleTypeIcons = new Dictionary<ModuleType, ImageSource>
        {
            [ModuleType.StandardModule] = ToImageSource(Icons.ObjectModule),
            [ModuleType.ClassModule] = ToImageSource(Icons.ObjectClass),
            [ModuleType.ClassModulePrivate] = ToImageSource(Icons.ObjectClassPrivate),
            [ModuleType.ClassModulePredeclared] = ToImageSource(Icons.ObjectClassPredeclared),
            [ModuleType.ClassModuleInterface] = ToImageSource(Icons.ObjectInterface),
            [ModuleType.UserFormModule] = ToImageSource(Icons.ProjectForm),
        };

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return null!;
            }

            var moduleType = (ModuleType)value;
            if (ModuleTypeIcons.TryGetValue(moduleType, out var icon))
            {
                return icon;
            }

            return null!;
        }
    }
}