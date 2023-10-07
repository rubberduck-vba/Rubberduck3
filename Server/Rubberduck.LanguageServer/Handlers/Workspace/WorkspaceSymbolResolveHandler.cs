using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.LanguageServer.Handlers.Workspace
{
    public class WorkspaceSymbolResolveHandler : WorkspaceSymbolResolveHandlerBase
    {
        public async override Task<WorkspaceSymbol> Handle(WorkspaceSymbol request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            // TODO resolve the missing symbol information
            return await Task.FromResult(request);
        }
    }
}