using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.General;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.ServerPlatform;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.Editor.RPC.EditorServer.Handlers.Lifecycle
{
    public class ShutdownHandler : ShutdownHandlerBase
    {
        private readonly ServerPlatformServiceHelper _service;
        private readonly IServerStateWriter _serverState;

        public ShutdownHandler(ServerPlatformServiceHelper service, IServerStateWriter serverState)
        {
            _service = service;
            _serverState = serverState;
        }

        public async override Task<Unit> Handle(ShutdownParams request, CancellationToken cancellationToken)
        {
            _service.LogTrace("Received Shutdown notification.");

            cancellationToken.ThrowIfCancellationRequested();

            _service.RunAction(() =>
            {
                _service.LogInformation("Shutting down...");
                _serverState.Shutdown(request);
            }, nameof(ShutdownHandler));

            return await Task.FromResult(Unit.Value);
        }
    }
}