using System;

namespace Rubberduck.SettingsProvider.Model
{
    [Flags]
    public enum SettingTags
    {
        None = 0,
        /// <summary>
        /// Tags a setting as commonly used for quicker access.
        /// </summary>
        Common = 1,
        /// <summary>
        /// Tags a setting as experimental.
        /// </summary>
        Experimental = 2,
        /// <summary>
        /// Tags a setting that should probably be left alone.
        /// </summary>
        ReadOnlyRecommended = 4,
        /// <summary>
        /// Tags a setting intended for advanced users and/or Rubberduck developers.
        /// </summary>
        Advanced = 8,
        /// <summary>
        /// Tags a setting that does not need a corresponding UI template, e.g. first-startup flags.
        /// </summary>
        Hidden = 128,
    }
}
