using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model.LanguageClient;
using Rubberduck.SettingsProvider.Model.ServerStartup;
using System.Collections.Generic;
using System.Linq;

namespace Rubberduck.SettingsProvider.Model
{
    public record class GeneralSettings : SettingGroup, IDefaultSettingsProvider<GeneralSettings>
    {
        // TODO localize
        private static readonly string _description = "Configures general-scope options that aren't necessarily tied to a specific Rubberduck component.";
        private static readonly IRubberduckSetting[] DefaultSettings =
            new IRubberduckSetting[]
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

        public GeneralSettings(GeneralSettings original, IEnumerable<IRubberduckSetting>? settings) 
            : base(original) 
        {
            Value = settings?.ToArray() ?? DefaultSettings;
        }

        public GeneralSettings(params IRubberduckSetting[] settings)
            : base(nameof(GeneralSettings), settings, DefaultSettings) { }

        public GeneralSettings(IEnumerable<IRubberduckSetting> settings)
            : base(nameof(GeneralSettings), settings, DefaultSettings) { }

        public string Locale => GetSetting<LocaleSetting>().Value;
        public LogLevel LogLevel => GetSetting<LogLevelSetting>().Value;
        public MessageTraceLevel TraceLevel => GetSetting<TraceLevelSetting>().Value;
        public bool ShowSplash => GetSetting<ShowSplashSetting>().Value;
        public bool DisableInitialLogLevelReset => GetSetting<DisableInitialLogLevelResetSetting>().Value;
        public bool DisableInitialLegacyIndenterCheck => GetSetting<DisableInitialLegacyIndenterCheckSetting>().Value;
        public string[] DisabledMessageKeys => GetSetting<DisabledMessageKeysSetting>().Value;

        public static GeneralSettings Default { get; } = new(DefaultSettings);
        GeneralSettings IDefaultSettingsProvider<GeneralSettings>.Default => Default;
    }
}
