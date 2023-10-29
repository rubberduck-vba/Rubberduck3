using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model.LanguageClient;
using Rubberduck.SettingsProvider.Model.LanguageServer;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Rubberduck.SettingsProvider.Model
{
    public record class RubberduckSettings : TypedSettingGroup, IDefaultSettingsProvider<RubberduckSettings>
    {
        // TODO localize
        private static readonly string _description = "A container for all configuration settings.";
        private static readonly RubberduckSetting[] DefaultSettings =
            new RubberduckSetting[]
            {
                new GeneralSettings(),
                new LanguageClientSettings(),
                new LanguageServerSettings(),
                new UpdateServerSettings(),
                new TelemetryServerSettings(),
            };

        public RubberduckSettings()
            : base(nameof(RubberduckSettings), DefaultSettings, DefaultSettings) { }

        public RubberduckSettings(params RubberduckSetting[] settings)
            : base(nameof(RubberduckSettings), settings, DefaultSettings) { }

        public RubberduckSettings(IEnumerable<RubberduckSetting> settings)
            : base(nameof(RubberduckSettings), settings, DefaultSettings) { }

        public RubberduckSettings(RubberduckSettings original, IEnumerable<RubberduckSetting> settings)
            : base(original)
        {
            Value = settings.ToArray();
        }

        [JsonIgnore]
        public GeneralSettings GeneralSettings => GetSetting<GeneralSettings>();
        [JsonIgnore]
        public LanguageClientSettings LanguageClientSettings => GetSetting<LanguageClientSettings>();
        [JsonIgnore]
        public LanguageServerSettings LanguageServerSettings => GetSetting<LanguageServerSettings>();
        [JsonIgnore]
        public UpdateServerSettings UpdateServerSettings => GetSetting<UpdateServerSettings>();
        [JsonIgnore]
        public TelemetryServerSettings TelemetryServerSettings => GetSetting<TelemetryServerSettings>();

        public static RubberduckSettings Default { get; } = new(DefaultSettings);
        RubberduckSettings IDefaultSettingsProvider<RubberduckSettings>.Default => Default;
    }
}
