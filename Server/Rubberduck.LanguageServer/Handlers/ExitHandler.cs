using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.General;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.LanguageServer.Handlers
{
    public class ExitHandler : ExitHandlerBase
    {
        public async override Task<Unit> Handle(ExitParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            // TODO
            return await Task.FromResult(Unit.Value);
        }
    }
}