using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using Rubberduck.InternalApi.Settings;
using Rubberduck.ServerPlatform;
using Rubberduck.SettingsProvider.Model;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.Editor.RPC.EditorServer.Handlers.Workspace
{
    public class DidChangeConfigurationHandler : DidChangeConfigurationHandlerBase
    {
        private readonly ServerPlatformServiceHelper _service;
        private readonly ISettingsChangedHandler<RubberduckSettings> _settingsChangedHandler;

        public DidChangeConfigurationHandler(ServerPlatformServiceHelper service, ISettingsChangedHandler<RubberduckSettings> settingsChangedHandler) 
        {
            _service = service;
            _settingsChangedHandler = settingsChangedHandler;
        }

        public override async Task<Unit> Handle(DidChangeConfigurationParams request, CancellationToken cancellationToken)
        {
            _service.TryRunAction(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                var settings = request.Settings?.ToObject<RubberduckSettings>()
                    ?? throw new System.InvalidOperationException("ConfigurationParams contains no 'Settings' object.");

                _settingsChangedHandler.OnSettingsChanged(settings);

            }, nameof(DidChangeConfigurationHandler));

            return await Task.FromResult(Unit.Value);
        }
    }
}