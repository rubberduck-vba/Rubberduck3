using Rubberduck.InternalApi.Settings;
using System;
using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.Settings.Model.LanguageClient;

/// <summary>
/// Configures LSP (Language Server Protocol) client options. The LSP client runs in the Rubberduck Editor process.
/// </summary>
public record class LanguageClientSettings : TypedSettingGroup, IDefaultSettingsProvider<LanguageClientSettings>
{
    private static readonly RubberduckSetting[] DefaultSettings =
        [
            new RequireAddInHostSetting { Value = RequireAddInHostSetting.DefaultSettingValue },
            new RequireSavedHostSetting { Value = RequireSavedHostSetting.DefaultSettingValue },
            new LanguageClientStartupSettings { Value = LanguageClientStartupSettings.DefaultSettings },
            new ExitNotificationDelaySetting { Value = ExitNotificationDelaySetting.DefaultSettingValue },
            WorkspaceSettings.Default,
        ];

    public LanguageClientSettings()
    {
        DefaultValue = DefaultSettings;
    }

    [JsonIgnore]
    public WorkspaceSettings WorkspaceSettings => GetSetting<WorkspaceSettings>() ?? WorkspaceSettings.Default;
    [JsonIgnore]
    public bool RequireAddInHost => GetSetting<RequireAddInHostSetting>()?.TypedValue ?? RequireAddInHostSetting.DefaultSettingValue;
    [JsonIgnore]
    public bool RequireSavedHost => GetSetting<RequireSavedHostSetting>()?.TypedValue ?? RequireSavedHostSetting.DefaultSettingValue;
    [JsonIgnore]
    public LanguageClientStartupSettings StartupSettings => GetSetting<LanguageClientStartupSettings>() ?? LanguageClientStartupSettings.Default;
    [JsonIgnore]
    public TimeSpan ExitNotificationDelay => GetSetting<ExitNotificationDelaySetting>()?.TypedValue ?? ExitNotificationDelaySetting.DefaultSettingValue;

    public static LanguageClientSettings Default { get; } = new() { Value = DefaultSettings, DefaultValue = DefaultSettings };
    LanguageClientSettings IDefaultSettingsProvider<LanguageClientSettings>.Default => Default;
}
