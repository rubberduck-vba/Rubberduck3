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
    public class TelemetryServerState : ServerState<TelemetryServerSettingsGroup>
    {
        public TelemetryServerState(
            ILogger<ServerState<TelemetryServerSettingsGroup>> logger,
            IHealthCheckService<TelemetryServerSettingsGroup> healthCheck)
            : base(logger, healthCheck)
        {
        }
    }
}