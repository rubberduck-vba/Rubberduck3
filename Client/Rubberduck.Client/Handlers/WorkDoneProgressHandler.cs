using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol;
using System.Threading.Tasks;
using MediatR;
using System.Threading;

namespace Rubberduck.Client.Handlers
{
    public class WorkDoneProgressHandler : ProgressHandlerBase
    {
        public override async Task<Unit> Handle(ProgressParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var token = request.Token;

            return await Task.FromResult(Unit.Value);
        }
    }
}
