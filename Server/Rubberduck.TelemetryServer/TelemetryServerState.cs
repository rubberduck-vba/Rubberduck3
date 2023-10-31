using Microsoft.Extensions.Logging;
using Rubberduck.ServerPlatform;
using Rubberduck.SettingsProvider.Model.TelemetryServer;

namespace Rubberduck.TelemetryServer
{
    public class TelemetryServerState : ServerState<TelemetryServerSettings, TelemetryServerStartupSettings>
    {
        public TelemetryServerState(
            ILogger<ServerState<TelemetryServerSettings, TelemetryServerStartupSettings>> logger,
            IHealthCheckService<TelemetryServerStartupSettings> healthCheck)
            : base(logger, healthCheck)
        {
        }
    }
}