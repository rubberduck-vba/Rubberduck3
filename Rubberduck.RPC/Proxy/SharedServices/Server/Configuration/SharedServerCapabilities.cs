using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.SharedServices.Console.Configuration;

namespace Rubberduck.RPC.Proxy.SharedServices.Server.Configuration
{
    /// <summary>
    /// Server configuration settings shared between implementations.
    /// </summary>
    public class SharedServerCapabilities
    {
        [RubberduckSP("consoleProvider")]
        public ServerConsoleOptions ConsoleOptions { get; set; }

        [RubberduckSP("telemetryProvider")]
        public TelemetryOptions TelemetryOptions { get; set; }
    }
}
