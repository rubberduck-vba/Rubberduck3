using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.LanguageServer.Handlers
{
    public class DidChangeConfigurationHandler : DidChangeConfigurationHandlerBase
    {
        public override async Task<Unit> Handle(DidChangeConfigurationParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            // TODO ACK client config changes

            return await Task.FromResult(Unit.Value);
        }
    }
}