using Rubberduck.InternalApi.ServerPlatform;
using Rubberduck.SettingsProvider.Model.ServerStartup;
using System;

namespace Rubberduck.SettingsProvider.Model
{
    public record class TelemetryServerStartupSettings : ServerStartupSettings
    {
        // TODO localize
        private static readonly string _description = "Configures the command-line startup options of the telemetry server.";

        public TelemetryServerStartupSettings()
            : base(nameof(TelemetryServerStartupSettings), _description)
        {
        }

        protected override string DefaultServerExecutablePath
            => @$"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Rubberduck\Telemetry\{ServerPlatformSettings.TelemetryServerExecutable}";

        protected override string DefaultServerPipeName => ServerPlatformSettings.TelemetryServerDefaultPipeName;
    }
}
