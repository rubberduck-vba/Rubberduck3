using Rubberduck.InternalApi.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.Settings.Model.Editor.Tools;

public record class ToolsSettings : TypedSettingGroup, IDefaultSettingsProvider<ToolsSettings>
{
    private static readonly RubberduckSetting[] DefaultSettings =
        [
            WorkspaceExplorerSettings.Default,
            ServerTraceSettings.Default,
        ];

    public ToolsSettings()
    {
        Value = DefaultValue = DefaultSettings;
    }

    public ToolsSettings(IDictionary<string, RubberduckSetting> settings) : this()
    {
        var defaultKeys = DefaultSettings.Select(e => e.Key).ToHashSet();
        Value = settings.Where(e => defaultKeys.Contains(e.Key));
    }

    [JsonIgnore]
    public WorkspaceExplorerSettings WorkspaceExplorerSettings => GetSetting<WorkspaceExplorerSettings>() ?? WorkspaceExplorerSettings.Default;
    [JsonIgnore]
    public ServerTraceSettings ServerTraceSettings => GetSetting<ServerTraceSettings>() ?? ServerTraceSettings.Default;

    [JsonIgnore]
    public IEnumerable<ToolWindowSettings> StartupToolWindows => TypedValue.OfType<ToolWindowSettings>().Where(e => e.ShowOnStartup);

    public static ToolsSettings Default { get; } = new ToolsSettings() { Value = DefaultSettings, DefaultValue = DefaultSettings };
    ToolsSettings IDefaultSettingsProvider<ToolsSettings>.Default => Default;
}
