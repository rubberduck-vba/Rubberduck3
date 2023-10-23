using Rubberduck.InternalApi.ServerPlatform;
using Rubberduck.SettingsProvider.Model.ServerStartup;

namespace Rubberduck.SettingsProvider.Model.LanguageClient
{
    public record class LanguageClientStartupSettings : ServerStartupSettings
    {
        // TODO localize
        private static readonly string _description = "Configures the command-line startup options of the language server.";

        public LanguageClientStartupSettings()
            : base(nameof(LanguageClientStartupSettings), _description)
        {
        }

        protected override string DefaultServerExecutablePath => ServerPlatformSettings.EditorServerExecutable;
        protected override string DefaultServerPipeName => ServerPlatformSettings.EditorServerDefaultPipeName;
    }
}
