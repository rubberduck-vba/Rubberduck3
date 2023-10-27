using Rubberduck.InternalApi.ServerPlatform;
using Rubberduck.SettingsProvider.Model.ServerStartup;
using System;

namespace Rubberduck.SettingsProvider.Model.LanguageServer
{
    public record class LanguageServerStartupSettings : ServerStartupSettings
    {
        // TODO localize
        private static readonly string _description = "Configures the command-line startup options of the language server.";

        public LanguageServerStartupSettings()
            : base(nameof(LanguageServerStartupSettings), _description)
        {
        }

        protected override string DefaultServerExecutablePath
            => @$"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Rubberduck\LanguageServer\{ServerPlatformSettings.TelemetryServerExecutable}";
        protected override string DefaultServerPipeName => ServerPlatformSettings.LanguageServerDefaultPipeName;
    }
}
