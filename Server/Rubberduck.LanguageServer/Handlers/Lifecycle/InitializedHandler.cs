using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.General;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.ServerPlatform;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.LanguageServer.Handlers
{
    public class InitializedHandler : LanguageProtocolInitializedHandlerBase
    {
        private readonly ServerPlatformServiceHelper _service;

        public InitializedHandler(ServerPlatformServiceHelper service) 
        {
            _service = service;
        }

        public async override Task<Unit> Handle(InitializedParams request, CancellationToken cancellationToken)
        {
            _service.LogInformation("Received Initialized request.");
            cancellationToken.ThrowIfCancellationRequested();

            return await Task.FromResult(Unit.Value);
        }
    }
}