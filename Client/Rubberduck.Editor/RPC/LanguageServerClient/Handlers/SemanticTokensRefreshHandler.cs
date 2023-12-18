using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Threading.Tasks;
using MediatR;
using System.Threading;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Rubberduck.Editor.RPC.LanguageServerClient.Handlers
{
    public class SemanticTokensRefreshHandler : SemanticTokensRefreshHandlerBase
    {
        public override async Task<Unit> Handle(SemanticTokensRefreshParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();


            // TODO
            /* The workspace/semanticTokens/refresh request is sent from the server to the client. 
             * Servers can use it to ask clients to refresh the editors for which this server provides semantic tokens. 
             * As a result the client should ask the server to recompute the semantic tokens for these editors. 
             * This is useful if a server detects a project wide configuration change which requires a re-calculation of all semantic tokens. 
             * Note that the client still has the freedom to delay the re-calculation of the semantic tokens if for example an editor is currently not visible.
            */
            return await Task.FromResult(Unit.Value);
        }
    }
}
