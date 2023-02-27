using Rubberduck.InternalApi.Common;
using Rubberduck.Settings;
using System;
using System.Linq;

namespace Rubberduck.Main.Extensions
{
    internal static class TypeExtensions
    {
        internal static bool NotDisabledOrExperimental(this Type type, GeneralSettings initialSettings)
        {
            return type.NotDisabled() && type.NotExperimental(initialSettings);
        }

        internal static bool NotExperimental(this Type type, GeneralSettings initialSettings)
        {
            var attribute = type.GetCustomAttributes(typeof(ExperimentalAttribute), false).FirstOrDefault();
            var resourceKey = (attribute as ExperimentalAttribute)?.ResourceKey ?? string.Empty;

            return attribute == null || initialSettings.EnableExperimentalFeatures.Any(a => a.Key == resourceKey && a.IsEnabled);
        }

        internal static bool NotDisabled(this Type type)
        {
            return !Attribute.IsDefined(type, typeof(DisabledAttribute));
        }

        internal static bool IsBasedOn(this Type type, Type allegedBase)
        {
            return allegedBase.IsAssignableFrom(type);
        }
    }
}
