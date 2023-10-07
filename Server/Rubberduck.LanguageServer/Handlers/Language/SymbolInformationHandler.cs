using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.LanguageServer.Handlers.Language
{
    public class SymbolInformationHandler : SymbolInformationHandlerBase
    {
        public async override Task<Container<SymbolInformation>?> Handle(SymbolInformationParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var items = new List<SymbolInformation>();
            // TODO

            var result = new Container<SymbolInformation>(items);
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