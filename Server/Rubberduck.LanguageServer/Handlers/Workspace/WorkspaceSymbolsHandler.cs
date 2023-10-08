using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.LanguageServer.Handlers.Workspace
{
    public class WorkspaceSymbolsHandler : WorkspaceSymbolsHandlerBase
    {
        public async override Task<Container<WorkspaceSymbol>?> Handle(WorkspaceSymbolParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var items = new List<WorkspaceSymbol>();
            // TODO

            var result = new Container<WorkspaceSymbol>(items);
            return await Task.FromResult(result);
        }

        protected override WorkspaceSymbolRegistrationOptions CreateRegistrationOptions(WorkspaceSymbolCapability capability, ClientCapabilities clientCapabilities)
        {
            return new WorkspaceSymbolRegistrationOptions
            {
                WorkDoneProgress = true,
                ResolveProvider = true
            };
        }
    }
}