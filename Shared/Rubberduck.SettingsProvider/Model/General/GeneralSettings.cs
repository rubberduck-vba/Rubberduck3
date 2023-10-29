using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model.LanguageClient;
using Rubberduck.SettingsProvider.Model.ServerStartup;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Rubberduck.SettingsProvider.Model
{
    public record class GeneralSettings : TypedSettingGroup, IDefaultSettingsProvider<GeneralSettings>
    {
        // TODO localize
        private static readonly string _description = "Configures general-scope options that aren't necessarily tied to a specific Rubberduck component.";
        private static readonly RubberduckSetting[] DefaultSettings =
            new RubberduckSetting[]
            {
                new LocaleSetting(),
                new LogLevelSetting(),
                new TraceLevelSetting(nameof(TraceLevelSetting), MessageTraceLevel.Verbose),
                new ShowSplashSetting(),
                new DisableInitialLogLevelResetSetting(),
                new DisableInitialLegacyIndenterCheckSetting(),
                new DisabledMessageKeysSetting(),
            };

        public GeneralSettings() 
            : base(nameof(GeneralSettings), DefaultSettings, DefaultSettings) { }

        public GeneralSettings(GeneralSettings original, IEnumerable<RubberduckSetting>? settings) 
            : base(original) 
        {
            Value = settings?.ToArray() ?? DefaultSettings;
        }

        public GeneralSettings(params RubberduckSetting[] settings)
            : base(nameof(GeneralSettings), settings, DefaultSettings) { }

        public GeneralSettings(IEnumerable<RubberduckSetting> settings)
            : base(nameof(GeneralSettings), settings, DefaultSettings) { }

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

        public static GeneralSettings Default { get; } = new(DefaultSettings);
        GeneralSettings IDefaultSettingsProvider<GeneralSettings>.Default => Default;
    }
}
