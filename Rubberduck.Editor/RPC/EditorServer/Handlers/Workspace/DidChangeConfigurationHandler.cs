using MediatR;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using Rubberduck.InternalApi.Common;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.Editor.RPC.EditorServer.Handlers.Workspace
{
    public class DidChangeConfigurationHandler : DidChangeConfigurationHandlerBase
    {
        private readonly ILogger _logger;
        private readonly ISettingsChangedHandler<RubberduckSettings> _settings;

        public DidChangeConfigurationHandler(ILogger<DidChangeConfigurationHandler> logger, ISettingsChangedHandler<RubberduckSettings> settings) 
        {
            _logger = logger;
            _settings = settings;
        }

        public override async Task<Unit> Handle(DidChangeConfigurationParams request, CancellationToken cancellationToken)
        {
            if (TimedAction.TryRun(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                var settings = request.Settings?.ToObject<RubberduckSettings>() 
                    ?? throw new System.InvalidOperationException("ConfigurationParams contains no 'Settings' object.");

                _settings.OnSettingsChanged(settings);

            }, out var elapsed, out var exception))
            {
                var level = _settings.Settings.GeneralSettings.TraceLevel.ToTraceLevel();
                _logger.LogPerformance(level, "Handled DidChangeConfiguration notification.", elapsed);
            }

            return await Task.FromResult(Unit.Value);
        }
    }
}