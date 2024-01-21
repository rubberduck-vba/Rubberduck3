using System.Collections.Generic;

namespace Rubberduck.InternalApi.Model
{
    public enum Accessibility
    {
        Implicit,
        Private,
        Friend,
        Public,
        Global,
        Static
    }

    public static class AccessibilityExtensions
    {
        /// <summary>
        /// Gets the string/token representation of an accessibility specifier.
        /// </summary>
        /// <remarks>Implicit accessibility being unspecified, yields an empty string.</remarks>
        public static string TokenString(this Accessibility access)
        {
            return access == Accessibility.Implicit ? string.Empty : access.ToString();
        }

        private static readonly Dictionary<Accessibility, Accessibility> _map = new()
        {
            [Accessibility.Implicit] = Accessibility.Public,
            [Accessibility.Private] = Accessibility.Private,
            [Accessibility.Friend] = Accessibility.Friend,
            [Accessibility.Public] = Accessibility.Public,
            [Accessibility.Global] = Accessibility.Public,
            [Accessibility.Static] = Accessibility.Static,
        };

        public static Accessibility Effective(this Accessibility access) => _map[access];
    }
}
