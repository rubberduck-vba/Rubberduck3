﻿using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model.ServerStartup;
using System;
using System.Text.Json.Serialization;

namespace Rubberduck.SettingsProvider.Model.UpdateServer
{
    /// <summary>
    /// Configures the update server settings.
    /// </summary>
    public class UpdateServerSettings : TypedSettingGroup, IDefaultSettingsProvider<UpdateServerSettings>
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
        public UpdateServerStartupSettings StartupSettings => GetSetting<UpdateServerStartupSettings>();
        [JsonIgnore]
        public MessageTraceLevel TraceLevel => GetSetting<TraceLevelSetting>().TypedValue;
        [JsonIgnore]
        public bool IsEnabled => GetSetting<IsUpdateServerEnabledSetting>().TypedValue;
        [JsonIgnore]
        public bool IncludePreReleases => GetSetting<IncludePreReleasesSetting>().TypedValue;
        [JsonIgnore]
        public Uri RubberduckWebApiBaseUrl => GetSetting<WebApiBaseUrlSetting>().TypedValue;

        public static UpdateServerSettings Default { get; } = new() { Value = DefaultSettings };
        UpdateServerSettings IDefaultSettingsProvider<UpdateServerSettings>.Default => Default;
    }
}
