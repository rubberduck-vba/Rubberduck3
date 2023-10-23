using Microsoft.Extensions.Logging;
using Rubberduck.LanguagePlatform;
using Rubberduck.ServerPlatform;
using Rubberduck.SettingsProvider.Model;

namespace Rubberduck.TelemetryServer
{
    public class TelemetryServerState : ServerState<TelemetryServerSettingsGroup, TelemetryServerStartupSettings>
    {
        public TelemetryServerState(
            ILogger<ServerState<TelemetryServerSettingsGroup, TelemetryServerStartupSettings>> logger,
            IHealthCheckService<TelemetryServerStartupSettings> healthCheck)
            : base(logger, healthCheck)
        {
        }
    }
}