using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using System.Threading.Tasks;
using MediatR;
using System.Threading;
using Rubberduck.ServerPlatform;

namespace Rubberduck.Editor.RPC.LanguageServerClient.Handlers
{
    public class WorkDoneProgressCreateHandler : WorkDoneProgressCreateHandlerBase
    {
        private readonly ServerPlatformServiceHelper _service;

        public WorkDoneProgressCreateHandler(ServerPlatformServiceHelper service)
        {
            _service = service;
        }

        public override async Task<Unit> Handle(WorkDoneProgressCreateParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            var token = request.Token;

            return await Task.FromResult(Unit.Value);
        }
    }
}
