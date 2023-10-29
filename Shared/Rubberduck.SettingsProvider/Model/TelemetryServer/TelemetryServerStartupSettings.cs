using Rubberduck.InternalApi.ServerPlatform;
using Rubberduck.SettingsProvider.Model.ServerStartup;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rubberduck.SettingsProvider.Model
{
    public record class TelemetryServerStartupSettings : ServerStartupSettings
    {
        // TODO localize
        private static readonly string _description = "Configures the command-line startup options of the telemetry server.";
        private static readonly IRubberduckSetting[] DefaultSettings = GetDefaultSettings(nameof(TelemetryServerStartupSettings),
            ServerPlatformSettings.TelemetryServerDefaultPipeName,
            @$"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Rubberduck\Telemetry\{ServerPlatformSettings.TelemetryServerExecutable}");

        public TelemetryServerStartupSettings()
            : base(nameof(TelemetryServerStartupSettings), DefaultSettings, DefaultSettings) { }

        public TelemetryServerStartupSettings(params IRubberduckSetting[] settings)
            : base(nameof(TelemetryServerStartupSettings), settings, DefaultSettings) { }

        public TelemetryServerStartupSettings(IEnumerable<IRubberduckSetting> settings)
            : base(nameof(TelemetryServerStartupSettings), settings.ToArray(), DefaultSettings) { }
    }
}
