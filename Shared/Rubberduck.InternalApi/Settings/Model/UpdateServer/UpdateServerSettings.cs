using Rubberduck.InternalApi.Settings;
using Rubberduck.InternalApi.Settings.Model.ServerStartup;
using System;
using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.Settings.Model.UpdateServer;

/// <summary>
/// Configures the update server settings.
/// </summary>
public record class UpdateServerSettings : TypedSettingGroup, IDefaultSettingsProvider<UpdateServerSettings>
{
    public static RubberduckSetting[] DefaultSettings { get; } = new RubberduckSetting[]
    {
        new UpdateServerStartupSettings { Value = UpdateServerStartupSettings.DefaultSettings },
        new TraceLevelSetting { Value = TraceLevelSetting.DefaultSettingValue },
        new IsUpdateServerEnabledSetting { Value = IsUpdateServerEnabledSetting.DefaultSettingValue },
        new IncludePreReleasesSetting { Value = IncludePreReleasesSetting.DefaultSettingValue },
        new WebApiBaseUrlSetting { Value = WebApiBaseUrlSetting.DefaultSettingValue },
    };

    public UpdateServerSettings()
    {
        DefaultValue = DefaultSettings;
    }

    [JsonIgnore]
    public UpdateServerStartupSettings StartupSettings => GetSetting<UpdateServerStartupSettings>() ?? UpdateServerStartupSettings.Default;
    [JsonIgnore]
    public MessageTraceLevel TraceLevel => GetSetting<TraceLevelSetting>()?.TypedValue ?? TraceLevelSetting.DefaultSettingValue;
    [JsonIgnore]
    public bool IsEnabled => GetSetting<IsUpdateServerEnabledSetting>()?.TypedValue ?? IsUpdateServerEnabledSetting.DefaultSettingValue;
    [JsonIgnore]
    public bool IncludePreReleases => GetSetting<IncludePreReleasesSetting>()?.TypedValue ?? IncludePreReleasesSetting.DefaultSettingValue;
    [JsonIgnore]
    public Uri RubberduckWebApiBaseUrl => GetSetting<WebApiBaseUrlSetting>()?.TypedValue ?? WebApiBaseUrlSetting.DefaultSettingValue;

    public static UpdateServerSettings Default { get; } = new() { Value = DefaultSettings, DefaultValue = DefaultSettings };
    UpdateServerSettings IDefaultSettingsProvider<UpdateServerSettings>.Default => Default;
}
