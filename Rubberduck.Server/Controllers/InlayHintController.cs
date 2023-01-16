using AustinHarris.JsonRpc;
using Rubberduck.InternalApi.RPC;
using Rubberduck.InternalApi.RPC.LSP.Response;
using Rubberduck.RPC.Platform;
using System.Threading.Tasks;
using WebSocketSharp;

namespace Rubberduck.Server.Controllers
{
    public class InlayHintController : JsonRpcClient
    {
        public InlayHintController(WebSocket socket) : base(socket)
        {
        }

        [JsonRpcMethod(JsonRpcMethods.ResolveInlayHint)]
        public async Task<InlayHint> Resolve(InlayHint parameters)
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.ResolveInlayHint, parameters);
                var response = Request<InlayHint>(request);

                return response;
            });
        }
    }
}
