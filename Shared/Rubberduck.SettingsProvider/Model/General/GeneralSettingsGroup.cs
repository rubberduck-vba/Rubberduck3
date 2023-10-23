using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model.LanguageClient;
using System;
using System.Collections.Generic;

namespace Rubberduck.SettingsProvider.Model
{
    public record class GeneralSettingsGroup : SettingGroup, IDefaultSettingsProvider<GeneralSettingsGroup>
    {
        // TODO localize
        private static readonly string _description = "Configures general-scope options that aren't necessarily tied to a specific Rubberduck component.";

        public GeneralSettingsGroup() 
            : base(nameof(GeneralSettingsGroup)[..^5], _description)
        {
        }

        public static GeneralSettingsGroup Default { get; } = new();

        public string Locale => Values[nameof(Locale)];
        public bool ShowSplash => bool.Parse(Values[nameof(ShowSplash)]);
        public bool DisableInitialLegacyIndenterCheck => bool.Parse(Values[nameof(DisableInitialLegacyIndenterCheck)]);

        public LogLevel LogLevel => Enum.Parse<LogLevel>(Values[nameof(LogLevel)]);
        public bool DisableInitialLogLevelReset => bool.Parse(Values[nameof(DisableInitialLogLevelReset)]);
        public string[] DisabledMessageKeys => Values[nameof(DisabledMessageKeys)];


        protected override IEnumerable<RubberduckSetting> Settings { get; init; } = new RubberduckSetting[]
        {
            new LocaleSetting(),
            new LogLevelSetting(),
            new ShowSplashSetting(),
            new DisableInitialLogLevelResetSetting(),
            new DisableInitialLegacyIndenterCheckSetting(),
            new DisabledMessageKeysSetting(),
        };

        GeneralSettingsGroup IDefaultSettingsProvider<GeneralSettingsGroup>.Default => Default;
    }
}
