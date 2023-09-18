using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.LanguageServer.Handlers
{
    public class SetTraceHandler : SetTraceHandlerBase
    {
        public async override Task<Unit> Handle(SetTraceParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            // TODO
            
            return await Task.FromResult(Unit.Value);
        }
    }
}