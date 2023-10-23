using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model.LanguageClient;
using Rubberduck.SettingsProvider.Model.LanguageServer;

namespace Rubberduck.SettingsProvider.Model
{
    public record class RubberduckSettings : IDefaultSettingsProvider<RubberduckSettings>
    {
        public RubberduckSettings()
        {
        }

        public RubberduckSettings(GeneralSettingsGroup generalSettings, LanguageClientSettingsGroup languageClientSettings, LanguageServerSettingsGroup languageServerSettings, UpdateServerSettingsGroup updateServerSettings, TelemetryServerSettingsGroup telemetryServerSettings)
        {
            GeneralSettings = generalSettings;
            LanguageClientSettings = languageClientSettings;
            LanguageServerSettings = languageServerSettings;
            UpdateServerSettings = updateServerSettings;
            TelemetryServerSettings = telemetryServerSettings;
        }

        public GeneralSettingsGroup GeneralSettings { get; init; } = new();
        public LanguageClientSettingsGroup LanguageClientSettings { get; init; } = new();
        public LanguageServerSettingsGroup LanguageServerSettings { get; init; } = new();
        public UpdateServerSettingsGroup UpdateServerSettings { get; init; } = new();
        public TelemetryServerSettingsGroup TelemetryServerSettings { get; init; } = new();

        public static RubberduckSettings Default { get; } = new();

        RubberduckSettings IDefaultSettingsProvider<RubberduckSettings>.Default => Default;
    }
}
