using MediatR;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.General;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Common;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.ServerPlatform;
using Rubberduck.SettingsProvider;
using Rubberduck.SettingsProvider.Model;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.UpdateServer.Handlers.Lifecycle
{
    public class ShutdownHandler : ShutdownHandlerBase
    {
        private readonly ILogger _logger;
        private readonly ISettingsProvider<LanguageServerSettings> _settingsProvider;
        private readonly IServerStateWriter _serverState;

        public ShutdownHandler(ILogger<ShutdownHandler> logger, ISettingsProvider<LanguageServerSettings> settings, IServerStateWriter serverState)
        {
            _logger = logger;
            _settingsProvider = settings;
            _serverState = serverState;
        }

        public async override Task<Unit> Handle(ShutdownParams request, CancellationToken cancellationToken)
        {
            _logger.LogTrace("Received Shutdown notification.");

            cancellationToken.ThrowIfCancellationRequested();
            var traceLevel = _settingsProvider.Settings.TraceLevel.ToTraceLevel();

            if (TimedAction.TryRun(() =>
            {
                _logger.LogInformation("Setting shutdown server state...");
                _serverState.Shutdown(request);

            }, out var elapsed, out var exception))
            {
                _logger.LogPerformance(traceLevel, "Handled Shutdown notification.", elapsed);
            }
            else if (exception != null)
            {
                _logger.LogError(traceLevel, exception);
            }

            return await Task.FromResult(Unit.Value);
        }
    }
}