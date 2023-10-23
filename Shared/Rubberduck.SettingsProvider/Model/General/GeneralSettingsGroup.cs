using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model.LanguageClient;
using Rubberduck.SettingsProvider.Model.ServerStartup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Rubberduck.SettingsProvider.Model
{
    public record class GeneralSettingsGroup : SettingGroup, IDefaultSettingsProvider<GeneralSettingsGroup>
    {
        // TODO localize
        private static readonly string _description = "Configures general-scope options that aren't necessarily tied to a specific Rubberduck component.";
        private static readonly RubberduckSetting[] DefaultSettings =
            new RubberduckSetting[]
            {
                new LocaleSetting(),
                new LogLevelSetting(),
                new TraceLevelSetting(nameof(GeneralSettingsGroup), MessageTraceLevel.Verbose),
                new ShowSplashSetting(),
                new DisableInitialLogLevelResetSetting(),
                new DisableInitialLegacyIndenterCheckSetting(),
                new DisabledMessageKeysSetting(),
            };

        public GeneralSettingsGroup(GeneralSettingsGroup original, IEnumerable<RubberduckSetting>? settings = null) 
            : base(original) 
        {
            Name = original.Name;
            Description = original.Description;

            var values = original.Settings.ToDictionary(e => e.Name);
            if (settings != null)
            {
                foreach (var setting in settings)
                {
                    values[setting.Name] = setting;
                }
                settings = values.Values;
            }

            Settings = settings ?? DefaultSettings;
        }

        public GeneralSettingsGroup(IEnumerable<RubberduckSetting>? settings = null)
            : base(nameof(GeneralSettingsGroup)[..^5], _description)
        {
            Settings = settings ?? DefaultSettings;
        }

        public static GeneralSettingsGroup Default { get; } = new();

        public string Locale => Values[nameof(Locale)];
        public bool ShowSplash => bool.Parse(Values[nameof(ShowSplash)]);
        public bool DisableInitialLegacyIndenterCheck => bool.Parse(Values[nameof(DisableInitialLegacyIndenterCheck)]);

        public LogLevel LogLevel => Enum.Parse<LogLevel>(Values[nameof(LogLevelSetting)]);
        public MessageTraceLevel TraceLevel => Enum.Parse<MessageTraceLevel>(Values[nameof(TraceLevel)]);

        public bool DisableInitialLogLevelReset => bool.Parse(Values[nameof(DisableInitialLogLevelReset)]);
        public string[] DisabledMessageKeys => JsonSerializer.Deserialize<string[]>(Values[nameof(DisabledMessageKeys)]) ?? Array.Empty<string>();


        protected override IEnumerable<RubberduckSetting> Settings { get; init; }

        GeneralSettingsGroup IDefaultSettingsProvider<GeneralSettingsGroup>.Default => Default;
    }
}
