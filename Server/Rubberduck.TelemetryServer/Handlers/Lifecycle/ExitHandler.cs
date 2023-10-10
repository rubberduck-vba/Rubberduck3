using MediatR;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.General;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Common;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.SettingsProvider;
using Rubberduck.SettingsProvider.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.TelemetryServer.Handlers.Lifecycle
{
    public class ExitHandler : ExitHandlerBase
    {
        private readonly ILogger _logger;
        private readonly ISettingsProvider<TelemetryServerSettings> _settingsProvider;
        private readonly Func<TelemetryServerState> _stateProvider;

        public ExitHandler(ILogger<ExitHandler> logger, ISettingsProvider<TelemetryServerSettings> settingsProvider, Func<TelemetryServerState> state)
        {
            _logger = logger;
            _settingsProvider = settingsProvider;
            _stateProvider = state;
        }

        public async override Task<Unit> Handle(ExitParams request, CancellationToken cancellationToken)
        {
            _logger.LogTrace("Received Exit notification.");

            cancellationToken.ThrowIfCancellationRequested();
            var traceLevel = _settingsProvider.Settings.TraceLevel.ToTraceLevel();
            var state = _stateProvider.Invoke();

            if (TimedAction.TryRun(() =>
            {
                _logger.LogInformation("Handling exit notification...");
                Environment.Exit(0);
            }, out var elapsed, out var exception))
            {
                _logger.LogPerformance(traceLevel, "Handled Exit notification. Process should exit with code 0.", elapsed);
            }
            else if (exception != null)
            {
                _logger.LogError(traceLevel, exception);
            }

            return await Task.FromResult(Unit.Value);
        }
    }
}