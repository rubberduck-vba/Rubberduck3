using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model.ServerStartup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Rubberduck.SettingsProvider.Model.LanguageServer
{
    public interface ILanguageServerSettings
    {
        MessageTraceLevel TraceLevel { get; }
        LanguageServerStartupSettings StartupSettings { get; }
    }

    public record class LanguageServerSettingsGroup : SettingGroup, IDefaultSettingsProvider<LanguageServerSettingsGroup>, ILanguageServerSettings
    {
        // TODO localize
        private static readonly string _description = "Configures LSP (Language Server Protocol) server options.";
        private static readonly RubberduckSetting[] DefaultSettings =
            new RubberduckSetting[]
            {
                new TraceLevelSetting(nameof(TraceLevelSetting), MessageTraceLevel.Verbose),
                new LanguageServerStartupSettings(),
            };

        public LanguageServerSettingsGroup(LanguageServerSettingsGroup original, IEnumerable<RubberduckSetting>? settings = null)
            : base(original)
        {
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

        public LanguageServerSettingsGroup() : base(nameof(LanguageServerSettingsGroup), _description) { }

        public LanguageServerSettingsGroup(IEnumerable<RubberduckSetting> settings)
            : base(nameof(LanguageServerSettingsGroup), _description)
        {
            Settings = settings ?? DefaultSettings;

            StartupSettings = Settings.OfType<LanguageServerStartupSettings>().Single();
        }

        public MessageTraceLevel TraceLevel { get; init; }

        public LanguageServerStartupSettings StartupSettings { get; init; }

        public static LanguageServerSettingsGroup Default { get; } = new(DefaultSettings);
        LanguageServerSettingsGroup IDefaultSettingsProvider<LanguageServerSettingsGroup>.Default => Default;
    }
}
