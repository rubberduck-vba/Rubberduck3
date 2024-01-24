using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Settings.Model.TelemetryServer;
using Rubberduck.ServerPlatform;

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