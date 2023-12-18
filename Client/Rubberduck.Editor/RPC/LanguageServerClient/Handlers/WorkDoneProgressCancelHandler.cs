using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using System.Threading.Tasks;
using MediatR;
using System.Threading;

namespace Rubberduck.Editor.RPC.LanguageServerClient.Handlers
{
    public class WorkDoneProgressCancelHandler : WorkDoneProgressCancelHandlerBase
    {
        public override async Task<Unit> Handle(WorkDoneProgressCancelParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // _workDoneProgressService.Cancel(request.Token);

            return await Task.FromResult(Unit.Value);
        }
    }
}
