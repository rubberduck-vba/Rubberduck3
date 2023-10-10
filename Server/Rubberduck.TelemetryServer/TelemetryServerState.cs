using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.General;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.LanguagePlatform;
using Rubberduck.ServerPlatform;
using Rubberduck.SettingsProvider.Model;
using System.Collections.Generic;
using System.Threading;

namespace Rubberduck.TelemetryServer
{
    public class TelemetryServerState : ServerState<TelemetryServerSettings>
    {
        public TelemetryServerState(
            ILogger<ServerState<TelemetryServerSettings>> logger,
            IHealthCheckService<TelemetryServerSettings> healthCheck)
            : base(logger, healthCheck)
        {
        }
    }
}