using Rubberduck.InternalApi.Settings;
using System.Collections.Generic;
using System.Linq;

namespace Rubberduck.SettingsProvider.Model.Editor
{
    public interface IEditorSettings
    {
    }

    public record class EditorSettingsGroup : SettingGroup, IDefaultSettingsProvider<EditorSettingsGroup>, IEditorSettings
    {
        // TODO localize
        private static readonly string _description = "Configures Rubberduck Editor settings. ";
        private static readonly RubberduckSetting[] DefaultSettings =
            new RubberduckSetting[]
            {
                /*TODO
                 * These settings should be specific to the editor, e.g. theming, fonts/font sizes, etc.
                 * See LanguageClientSettings for editor settings related to its LSP client functionalities.
                 */
            };

        public EditorSettingsGroup() : this(DefaultSettings) { }

        public EditorSettingsGroup(IEnumerable<RubberduckSetting> settings)
            : base(nameof(EditorSettingsGroup), _description)
        {
            Settings = settings ?? DefaultSettings;
        }

        public EditorSettingsGroup(EditorSettingsGroup original, IEnumerable<RubberduckSetting> settings)
            : base(original)
        {
            var values = original.Settings.ToDictionary(e => e.Name);
            if (settings != null)
            {
                foreach (var setting in settings)
                {
                    values[setting.Name] = setting;
                }
                settings = values.Values;
            }

            Settings = settings ?? DefaultSettings;
        }

        public static EditorSettingsGroup Default { get; } = new(DefaultSettings);
        EditorSettingsGroup IDefaultSettingsProvider<EditorSettingsGroup>.Default => Default;
    }
}
