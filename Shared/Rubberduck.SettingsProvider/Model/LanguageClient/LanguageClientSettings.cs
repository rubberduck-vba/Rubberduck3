using Rubberduck.InternalApi.Settings;
using System;
using System.Text.Json.Serialization;

namespace Rubberduck.SettingsProvider.Model.LanguageClient
{
    /// <summary>
    /// Configures LSP (Language Server Protocol) client options. The LSP client runs in the Rubberduck Editor process.
    /// </summary>
    public record class LanguageClientSettings : TypedSettingGroup, IDefaultSettingsProvider<LanguageClientSettings>
    {
        private static readonly RubberduckSetting[] DefaultSettings =
            new RubberduckSetting[]
            {
                new RequireAddInHostSetting { Value = RequireAddInHostSetting.DefaultSettingValue },
                new RequireSavedHostSetting { Value = RequireSavedHostSetting.DefaultSettingValue },
                new LanguageClientStartupSettings { Value = LanguageClientStartupSettings.DefaultSettings },
                new ExitNotificationDelaySetting { Value = ExitNotificationDelaySetting.DefaultSettingValue },
            };

        public LanguageClientSettings()
        {
            DefaultValue = DefaultSettings;
        }

        [JsonIgnore]
        public WorkspaceSettings WorkspaceSettings => GetSetting<WorkspaceSettings>();
        [JsonIgnore]
        public bool RequireAddInHost => GetSetting<RequireAddInHostSetting>().TypedValue;
        [JsonIgnore]
        public bool RequireSavedHost => GetSetting<RequireSavedHostSetting>().TypedValue;
        [JsonIgnore]
        public LanguageClientStartupSettings StartupSettings => GetSetting<LanguageClientStartupSettings>();
        [JsonIgnore]
        public TimeSpan ExitNotificationDelay => GetSetting<ExitNotificationDelaySetting>().TypedValue;

        public static LanguageClientSettings Default { get; } = new() { Value = DefaultSettings, DefaultValue = DefaultSettings };
        LanguageClientSettings IDefaultSettingsProvider<LanguageClientSettings>.Default => Default;
    }
}
