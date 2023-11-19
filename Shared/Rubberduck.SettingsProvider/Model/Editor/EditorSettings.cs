using Rubberduck.InternalApi.Model;
using Rubberduck.InternalApi.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rubberduck.SettingsProvider.Model.Editor
{
    /// <summary>
    /// Configures Rubberduck Editor settings.
    /// </summary>
    public record class EditorSettings : TypedSettingGroup, IDefaultSettingsProvider<EditorSettings>
    {
        private static readonly RubberduckSetting[] DefaultSettings =
            new RubberduckSetting[]
            {
                /*TODO
                 * These settings should be specific to the editor, e.g. theming, fonts/font sizes, etc.
                 * See LanguageClientSettings for editor settings related to its LSP client functionalities.
                 */
            };

        /*
         * TODO expose each value in the setting group with a property:
         *  public SomeSetting SettingName => GetSetting<SomeSetting>().Value;
        */

        public EditorSettings()
        {
            DefaultValue = DefaultSettings;
        }

        public static EditorSettings Default { get; } = new() { Value = DefaultSettings };
        EditorSettings IDefaultSettingsProvider<EditorSettings>.Default => Default;
    }
}
