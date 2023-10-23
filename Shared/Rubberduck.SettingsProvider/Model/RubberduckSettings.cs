using Rubberduck.SettingsProvider.Model.LanguageClient;
using Rubberduck.SettingsProvider.Model.LanguageServer;

namespace Rubberduck.SettingsProvider.Model
{
    public record class RubberduckSettings
    {
        public GeneralSettingsGroup GeneralSettings { get; init; } = new();
        public LanguageClientSettingsGroup LanguageClientSettings { get; init; } = new();
        public LanguageServerSettingsGroup LanguageServerSettings { get; init; } = new();
        public UpdateServerSettingGroup UpdateServerSettings { get; init; } = new();
        public TelemetryServerSettingsGroup TelemetryServerSettings { get; init; } = new();

        public static RubberduckSettings Default { get; } = new();
    }
}
