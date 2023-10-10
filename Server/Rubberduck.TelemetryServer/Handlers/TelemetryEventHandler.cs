using MediatR;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using Rubberduck.ServerPlatform.Model.Telemetry;
using Rubberduck.SettingsProvider;
using Rubberduck.SettingsProvider.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.TelemetryServer.Handlers
{
    public class TelemetryEventHandler : TelemetryEventHandlerBase<TelemetryEventPayload>
    {
        private readonly ILogger _logger;
        private readonly ISettingsProvider<TelemetryServerSettings> _settingsProvider;

        public TelemetryEventHandler(ILogger<TelemetryEventHandler> logger, ISettingsProvider<TelemetryServerSettings> settings)
        {
            _logger = logger;
            _settingsProvider = settings;
        }

        public override Task<Unit> Handle(TelemetryEventPayload request, CancellationToken cancellationToken)
        {

            return Task.FromResult(Unit.Value);
        }
    }
}
