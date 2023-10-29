using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model.ServerStartup;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Rubberduck.SettingsProvider.Model.LanguageServer
{
    public record class LanguageServerSettings : TypedSettingGroup, IDefaultSettingsProvider<LanguageServerSettings>
    {
        // TODO localize
        private static readonly string _description = "Configures LSP (Language Server Protocol) server options.";
        private static readonly RubberduckSetting[] DefaultSettings =
            new RubberduckSetting[]
            {
                new TraceLevelSetting(nameof(TraceLevelSetting), MessageTraceLevel.Verbose),
                new LanguageServerStartupSettings(),
            };

        public LanguageServerSettings() 
            : base(nameof(LanguageServerSettings), DefaultSettings, DefaultSettings) { }

        public LanguageServerSettings(LanguageServerSettings original, IEnumerable<RubberduckSetting>? settings)
            : base(original)
        {
            Value = settings?.ToArray() ?? DefaultSettings;
        }

        public LanguageServerSettings(params RubberduckSetting[] settings)
            : base(nameof(LanguageServerSettings), settings, DefaultSettings) { }

        public LanguageServerSettings(IEnumerable<RubberduckSetting> settings)
            : base(nameof(LanguageServerSettings), settings, DefaultSettings) { }


        [JsonIgnore]
        public MessageTraceLevel TraceLevel => GetSetting<TraceLevelSetting>().TypedValue;
        [JsonIgnore]
        public LanguageServerStartupSettings StartupSettings => GetSetting<LanguageServerStartupSettings>();

        public static LanguageServerSettings Default { get; } = new(DefaultSettings);
        LanguageServerSettings IDefaultSettingsProvider<LanguageServerSettings>.Default => Default;
    }
}
