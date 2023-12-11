using Rubberduck.InternalApi.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Rubberduck.SettingsProvider.Model.Tools
{
    public record class ToolsSettings : TypedSettingGroup, IDefaultSettingsProvider<ToolsSettings>
    {
        private static readonly RubberduckSetting[] DefaultSettings =
            new RubberduckSetting[]
            {
                new WorkspaceExplorerSettings(),
            };

        public ToolsSettings() { }

        public ToolsSettings(IDictionary<string, RubberduckSetting> settings) : this()
        {
            var defaultKeys = DefaultSettings.Select(e => e.Key).ToHashSet();
            Value = settings.Where(e => defaultKeys.Contains(e.Key));
        }

        [JsonIgnore]
        WorkspaceExplorerSettings WorkspaceExplorerSettings => GetSetting<WorkspaceExplorerSettings>() ?? WorkspaceExplorerSettings.Default;

        public static ToolsSettings Default { get; } = new ToolsSettings() { Value = DefaultSettings, DefaultValue = DefaultSettings };
        ToolsSettings IDefaultSettingsProvider<ToolsSettings>.Default => Default;
    }
}
