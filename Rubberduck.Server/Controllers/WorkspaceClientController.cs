using Rubberduck.InternalApi.RPC.LSP.Parameters;
using Rubberduck.InternalApi.RPC.LSP.Response;
using System.Threading.Tasks;
using Rubberduck.RPC.Platform;
using WebSocketSharp;
using Rubberduck.InternalApi.RPC;

namespace Rubberduck.Server.Controllers
{
    public class WorkspaceClientController : JsonRpcClient
    {
        public WorkspaceClientController(WebSocket socket) : base(socket)
        {
        }

        public async Task<ApplyWorkspaceEditResult> ApplyEdit(ApplyWorkspaceEditParams parameters)
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.ApplyWorkspaceEdit, parameters);
                var response = Request<ApplyWorkspaceEditResult>(request);

                return response;
            });
        }

        public async Task RefreshCodeLens()
        {
            await Task.Run(() => 
            {
                var request = CreateRequest(JsonRpcMethods.RefreshCodeLens, null);
                Notify(request);
            });
        }

        public async Task RefreshDiagnostics()
        {
            await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.RefreshDiagnostics, null);
                Notify(request);
            });
        }

        public async Task RefreshInlayHints()
        {
            await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.RefreshInlayHints, null);
                Notify(request);
            });
        }

        public async Task RefreshSemanticTokens()
        {
            await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.RefreshSemanticTokens, null);
                Notify(request);
            });
        }
    }
}
