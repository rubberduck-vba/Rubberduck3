using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Threading.Tasks;
using MediatR;
using System.Threading;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using Microsoft.Extensions.Logging;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.SettingsProvider;
using Rubberduck.InternalApi.Extensions;
using System.Diagnostics;

namespace Rubberduck.Client.Handlers
{
    public class LogTraceHandler : LogTraceHandlerBase
    {
        private readonly ILogger<WorkspaceFoldersHandler> _logger;
        private readonly ISettingsProvider<LanguageServerSettings> _settingsProvider;

        TraceLevel TraceLevel => _settingsProvider.Settings.TraceLevel.ToTraceLevel();

        public LogTraceHandler(ILogger<WorkspaceFoldersHandler> logger, ISettingsProvider<LanguageServerSettings> settingsProvider)
        {
            _logger = logger;
            _settingsProvider = settingsProvider;
        }

        public override async Task<Unit> Handle(LogTraceParams request, CancellationToken cancellationToken)
        {
            _logger.LogDebug(TraceLevel, "Received LogTrace request.", string.Empty);
            cancellationToken.ThrowIfCancellationRequested();
            if (TraceLevel != TraceLevel.Off)
            {
            }

            return await Task.FromResult(Unit.Value);
        }
    }
}
