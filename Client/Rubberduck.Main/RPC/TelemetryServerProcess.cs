using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.ServerPlatform;

namespace Rubberduck.Main.RPC
{
    public class TelemetryServerProcess : ServerProcess
    {
        public TelemetryServerProcess(ILogger logger)
            : base(logger)
        {
        }

        protected override string ExecutableFileName { get; } = ServerPlatformSettings.TelemetryServerExecutable;
    }
}