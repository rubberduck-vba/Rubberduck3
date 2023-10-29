using Rubberduck.InternalApi.Settings;
using System.Collections.Generic;
using System.Linq;

namespace Rubberduck.SettingsProvider.Model.Editor
{
    public interface IEditorSettings
    {
    }

    public record class EditorSettings : SettingGroup, IDefaultSettingsProvider<EditorSettings>, IEditorSettings
    {
        // TODO localize
        private static readonly string _description = "Configures Rubberduck Editor settings. ";
        private static readonly IRubberduckSetting[] DefaultSettings =
            new IRubberduckSetting[]
            {
                /*TODO
                 * These settings should be specific to the editor, e.g. theming, fonts/font sizes, etc.
                 * See LanguageClientSettings for editor settings related to its LSP client functionalities.
                 */
            };

        public EditorSettings() 
            : base(nameof(EditorSettings), DefaultSettings, DefaultSettings) { }

        public EditorSettings(params IRubberduckSetting[] settings)
            : base(nameof(EditorSettings), settings, DefaultSettings) { }

        public EditorSettings(IEnumerable<IRubberduckSetting> settings)
            : base(nameof(EditorSettings), settings, DefaultSettings) { }

        public EditorSettings(EditorSettings original, IEnumerable<IRubberduckSetting> settings)
            : base(original)
        {
            Value = settings.ToArray();
        }

        /*
         * TODO expose each value in the setting group with a property:
         *  public SomeSetting SettingName => GetSetting<SomeSetting>().Value;
        */

        public static EditorSettings Default { get; } = new(DefaultSettings);
        EditorSettings IDefaultSettingsProvider<EditorSettings>.Default => Default;
    }
}
