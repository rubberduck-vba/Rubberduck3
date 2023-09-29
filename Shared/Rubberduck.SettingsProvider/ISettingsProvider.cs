using System;
using System.Threading.Tasks;

namespace Rubberduck.SettingsProvider
{
    public interface ISettingsProvider<TSettings>
        where TSettings : struct
    {
        (Guid Token, TSettings Settings) Value { get; }
        bool TrySetValue(TSettings value, Guid token);
        bool TryGetValue(Guid token, out TSettings value);

        /// <summary>
        /// Gets the token mapped to the default TSettings configuration.
        /// </summary>
        Guid DefaultToken { get; }

        /// <summary>
        /// Gets the token mapped to the current TSettings configuration.
        /// </summary>
        Guid CurrentToken { get; }

        /// <summary>
        /// Clears cached settings and tokens, resetting to defaults.
        /// </summary>
        void ClearCache();
    }
}
