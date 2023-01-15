using AustinHarris.JsonRpc;
using Rubberduck.InternalApi.RPC.LSP;
using Rubberduck.InternalApi.RPC.LSP.Response;
using Rubberduck.RPC.Platform;
using System.Threading.Tasks;
using WebSocketSharp;

namespace Rubberduck.Server.Controllers
{
    public class WorkspaceSymbolController : JsonRpcClient
    {
        public WorkspaceSymbolController(WebSocket socket) : base(socket)
        {
        }

        [JsonRpcMethod(JsonRpcMethods.ResolveWorkspaceSymbol)]
        public async Task<WorkspaceSymbol> Resolve(WorkspaceSymbol parameters)
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.ResolveWorkspaceSymbol, parameters);
                var response = Request<WorkspaceSymbol>(request);

                return response;
            });
        }
    }
}
