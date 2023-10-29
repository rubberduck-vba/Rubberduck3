using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model.LanguageClient;
using Rubberduck.SettingsProvider.Model.LanguageServer;
using System.Collections.Generic;
using System.Linq;

namespace Rubberduck.SettingsProvider.Model
{
    public record class RubberduckSettings : SettingGroup, IDefaultSettingsProvider<RubberduckSettings>
    {
        // TODO localize
        private static readonly string _description = "A container for all configuration settings.";
        private static readonly IRubberduckSetting[] DefaultSettings =
            new IRubberduckSetting[]
            {
                new GeneralSettings(),
                new LanguageClientSettings(),
                new LanguageServerSettings(),
                new UpdateServerSettingsGroup(),
                new TelemetryServerSettings(),
            };

        public RubberduckSettings()
            : base(nameof(RubberduckSettings), DefaultSettings, DefaultSettings) { }

        public RubberduckSettings(params IRubberduckSetting[] settings)
            : base(nameof(RubberduckSettings), settings, DefaultSettings) { }

        public RubberduckSettings(IEnumerable<IRubberduckSetting> settings)
            : base(nameof(RubberduckSettings), settings, DefaultSettings) { }

        public RubberduckSettings(RubberduckSettings original, IEnumerable<IRubberduckSetting> settings)
            : base(original)
        {
            Value = settings.ToArray();
        }

        public GeneralSettings GeneralSettings => GetSetting<GeneralSettings>();
        public LanguageClientSettings LanguageClientSettings => GetSetting<LanguageClientSettings>();
        public LanguageServerSettings LanguageServerSettings => GetSetting<LanguageServerSettings>();
        public UpdateServerSettingsGroup UpdateServerSettings => GetSetting<UpdateServerSettingsGroup>();
        public TelemetryServerSettings TelemetryServerSettings => GetSetting<TelemetryServerSettings>();

        public static RubberduckSettings Default { get; } = new(DefaultSettings);
        RubberduckSettings IDefaultSettingsProvider<RubberduckSettings>.Default => Default;
    }
}
