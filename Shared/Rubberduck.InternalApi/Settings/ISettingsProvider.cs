using System;

namespace Rubberduck.InternalApi.Settings
{
    /// <summary>
    /// A service that caches and exposes the current <c>TSettings</c> value.
    /// </summary>
    /// <typeparam name="TSettings"></typeparam>
    public interface ISettingsProvider<TSettings>
        where TSettings : struct
    {
        /// <summary>
        /// Gets the currently applicable settings.
        /// </summary>
        TSettings Settings { get; }

        /// <summary>
        /// Clears cached settings and forces a reload from file.
        /// </summary>
        void ClearCache();

        /// <summary>
        /// Notifies listeners when settings are modified.
        /// </summary>
        event EventHandler<SettingsChangedEventArgs<TSettings>> SettingsChanged;
    }
}
