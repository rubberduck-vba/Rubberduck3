using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using System.Threading.Tasks;
using MediatR;
using System.Threading;

namespace Rubberduck.Client.Handlers
{
    public class WorkDoneProgressCreateHandler : WorkDoneProgressCreateHandlerBase
    {
        public override async Task<Unit> Handle(WorkDoneProgressCreateParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var token = request.Token;

            return await Task.FromResult(Unit.Value);
        }
    }
}
