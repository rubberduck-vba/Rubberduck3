using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Threading.Tasks;
using System.Threading;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Rubberduck.Client.Handlers
{
    public class ApplyWorkspaceEditHandler : ApplyWorkspaceEditHandlerBase
    {
        public override async Task<ApplyWorkspaceEditResponse> Handle(ApplyWorkspaceEditParams request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var response = new ApplyWorkspaceEditResponse();
            // TODO
            /*
             * The workspace/applyEdit request is sent from the server to the client to modify resource on the client side.
            */

            return await Task.FromResult(response);
        }
    }
}
