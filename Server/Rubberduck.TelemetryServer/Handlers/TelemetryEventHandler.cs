using MediatR;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using Rubberduck.ServerPlatform.Model.Telemetry;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.InternalApi.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using Rubberduck.InternalApi.Common;
using Rubberduck.InternalApi.Settings;

namespace Rubberduck.TelemetryServer.Handlers
{
    public class TelemetryEventHandler : TelemetryEventHandlerBase<TelemetryEventPayload>
    {
        private readonly ILogger _logger;
        private readonly ISettingsProvider<TelemetryServerSettingsGroup> _settingsProvider;
        private readonly ITelemetryService _telemetryService;

        public TelemetryEventHandler(ILogger<TelemetryEventHandler> logger, ISettingsProvider<TelemetryServerSettingsGroup> settings,
            ITelemetryService service)
        {
            _logger = logger;
            _settingsProvider = settings;
            _telemetryService = service;
        }

        public override Task<Unit> Handle(TelemetryEventPayload request, CancellationToken cancellationToken)
        {
            var trace = _settingsProvider.Settings.TraceLevel.ToTraceLevel();
            _logger.LogTrace(trace, "Received TelemetryEvent payload.", JsonSerializer.Serialize(request));
            if (TimedAction.TryRun(() =>
            {
                _telemetryService.Enqueue(request);
            }, out var elapsed, out var exception))
            {
                _logger.LogPerformance(trace, "Enqueued telemetry payload.", elapsed);
            }
            else if (exception is not null)
            {
                _logger.LogError(trace, exception);
            }
            return Task.FromResult(Unit.Value);
        }
    }
}
