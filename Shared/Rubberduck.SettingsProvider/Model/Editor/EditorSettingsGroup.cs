using Rubberduck.InternalApi.Settings;
using System.Collections.Generic;

namespace Rubberduck.SettingsProvider.Model.Editor
{
    public record class EditorSettingsGroup : SettingGroup, IDefaultSettingsProvider<EditorSettingsGroup>
    {
        // TODO localize
        private static readonly string _description = "Configures Rubberduck Editor settings.";

        public EditorSettingsGroup() : base(nameof(EditorSettingsGroup), _description)
        {
        }

        protected override IEnumerable<RubberduckSetting> Settings { get; init; } = new RubberduckSetting[]
        {
            // TODO go crazy
        };

        public static EditorSettingsGroup Default { get; } = new();
        EditorSettingsGroup IDefaultSettingsProvider<EditorSettingsGroup>.Default => Default;
    }
}
