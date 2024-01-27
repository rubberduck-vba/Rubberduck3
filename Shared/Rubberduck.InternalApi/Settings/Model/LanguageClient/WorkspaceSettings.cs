using Rubberduck.InternalApi.Settings;
using System;
using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.Settings.Model.LanguageClient;

public record class WorkspaceSettings : TypedSettingGroup, IDefaultSettingsProvider<WorkspaceSettings>
{
    private static readonly RubberduckSetting[] DefaultSettings =
        new RubberduckSetting[]
        {
            new DefaultWorkspaceRootSetting { Value = DefaultWorkspaceRootSetting.DefaultSettingValue },
            new RequireDefaultWorkspaceRootHostSetting { Value = RequireDefaultWorkspaceRootHostSetting.DefaultSettingValue },
            new EnableUncWorkspacesSetting { Value = EnableUncWorkspacesSetting.DefaultSettingValue },
            new EnableFileSystemWatchersSetting{ Value = EnableFileSystemWatchersSetting.DefaultSettingValue },
        };

    public WorkspaceSettings()
    {
        DefaultValue = DefaultSettings;
    }

    [JsonIgnore]
    public Uri DefaultWorkspaceRoot => GetSetting<DefaultWorkspaceRootSetting>()?.TypedValue ?? DefaultWorkspaceRootSetting.DefaultSettingValue;
    [JsonIgnore]
    public bool RequireDefaultWorkspaceRootHost => GetSetting<RequireDefaultWorkspaceRootHostSetting>()?.TypedValue ?? RequireDefaultWorkspaceRootHostSetting.DefaultSettingValue;
    [JsonIgnore]
    public bool EnableUncWorkspaces => GetSetting<EnableUncWorkspacesSetting>()?.TypedValue ?? EnableUncWorkspacesSetting.DefaultSettingValue;
    [JsonIgnore]
    public bool EnableFileSystemWatchers => GetSetting<EnableFileSystemWatchersSetting>()?.TypedValue ?? EnableFileSystemWatchersSetting.DefaultSettingValue;

    public static WorkspaceSettings Default { get; } = new() { Value = DefaultSettings, DefaultValue = DefaultSettings };
    WorkspaceSettings IDefaultSettingsProvider<WorkspaceSettings>.Default => Default;
}
