using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using Rubberduck.ServerPlatform;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.LanguageServer.Handlers.Lifecycle
{
    public class SetTraceHandler : SetTraceHandlerBase
    {
        private readonly ServerPlatformServiceHelper _service;
        private readonly IServerStateWriter _state;

        public SetTraceHandler(ServerPlatformServiceHelper service, IServerStateWriter state)
        {
            _service = service;
            _state = state;
        }

        public async override Task<Unit> Handle(SetTraceParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _service.LogTrace("Received SetTrace request.");

            _service.TryRunAction(() =>
            {
                _state.SetTraceLevel(request.Value);
            }, nameof(SetTraceHandler));

            return await Task.FromResult(Unit.Value);
        }
    }
}