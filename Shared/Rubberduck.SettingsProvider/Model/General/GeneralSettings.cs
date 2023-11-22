using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model.LanguageClient;
using System;
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
                new ShowSplashSetting { Value = ShowSplashSetting.DefaultSettingValue },
                new DisableInitialLegacyIndenterCheckSetting { Value = DisableInitialLegacyIndenterCheckSetting.DefaultSettingValue },
                new DisabledMessageKeysSetting { Value = DisabledMessageKeysSetting.DefaultSettingValue },
                new TemplatesLocationSetting { Value = TemplatesLocationSetting.DefaultSettingValue },
            };

        public GeneralSettings()
        {
        }

        [JsonIgnore]
        public string Locale => GetSetting<LocaleSetting>()?.TypedValue ?? LocaleSetting.DefaultSettingValue;
        [JsonIgnore]
        public bool ShowSplash => GetSetting<ShowSplashSetting>()?.TypedValue ?? ShowSplashSetting.DefaultSettingValue;
        [JsonIgnore]
        public bool DisableInitialLegacyIndenterCheck => GetSetting<DisableInitialLegacyIndenterCheckSetting>()?.TypedValue ?? DisableInitialLegacyIndenterCheckSetting.DefaultSettingValue;
        [JsonIgnore]
        public string[] DisabledMessageKeys => GetSetting<DisabledMessageKeysSetting>()?.TypedValue ?? DisabledMessageKeysSetting.DefaultSettingValue;
        [JsonIgnore]
        public Uri TemplatesLocation => GetSetting<TemplatesLocationSetting>()?.TypedValue ?? TemplatesLocationSetting.DefaultSettingValue;

        public static GeneralSettings Default { get; } = new GeneralSettings() { Value = DefaultSettings, DefaultValue = DefaultSettings };
        GeneralSettings IDefaultSettingsProvider<GeneralSettings>.Default => Default;
    }
}
