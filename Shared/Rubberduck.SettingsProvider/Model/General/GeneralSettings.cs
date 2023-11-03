using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model.LanguageClient;
using Rubberduck.SettingsProvider.Model.ServerStartup;
using System.Text.Json.Serialization;

namespace Rubberduck.SettingsProvider.Model.General
{
    /// <summary>
    /// Configures general-scope options that aren't necessarily tied to a specific Rubberduck component.
    /// </summary>
    public record class GeneralSettings : TypedSettingGroup, IDefaultSettingsProvider<GeneralSettings>
    {
        private static readonly RubberduckSetting[] DefaultSettings =
            new RubberduckSetting[]
            {
                new LocaleSetting { Value = LocaleSetting.DefaultSettingValue },
                new LogLevelSetting { Value = LogLevelSetting.DefaultSettingValue },
                new TraceLevelSetting { Value = TraceLevelSetting.DefaultSettingValue },
                new ShowSplashSetting { Value = ShowSplashSetting.DefaultSettingValue },
                new DisableInitialLogLevelResetSetting { Value = DisableInitialLogLevelResetSetting.DefaultSettingValue },
                new DisableInitialLegacyIndenterCheckSetting { Value = DisableInitialLegacyIndenterCheckSetting.DefaultSettingValue },
                new DisabledMessageKeysSetting { Value = DisabledMessageKeysSetting.DefaultSettingValue },
            };

        public GeneralSettings()
        {
        }

        [JsonIgnore]
        public string Locale => GetSetting<LocaleSetting>().TypedValue;
        [JsonIgnore]
        public LogLevel LogLevel => GetSetting<LogLevelSetting>().TypedValue;
        [JsonIgnore]
        public MessageTraceLevel TraceLevel => GetSetting<TraceLevelSetting>().TypedValue;
        [JsonIgnore]
        public bool ShowSplash => GetSetting<ShowSplashSetting>().TypedValue;
        [JsonIgnore]
        public bool DisableInitialLogLevelReset => GetSetting<DisableInitialLogLevelResetSetting>().TypedValue;
        [JsonIgnore]
        public bool DisableInitialLegacyIndenterCheck => GetSetting<DisableInitialLegacyIndenterCheckSetting>().TypedValue;
        [JsonIgnore]
        public string[] DisabledMessageKeys => GetSetting<DisabledMessageKeysSetting>().TypedValue;

        public static GeneralSettings Default { get; } = new GeneralSettings() { Value = DefaultSettings, DefaultValue = DefaultSettings };
        GeneralSettings IDefaultSettingsProvider<GeneralSettings>.Default => Default;
    }
}
