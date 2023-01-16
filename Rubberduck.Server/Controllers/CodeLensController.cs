using AustinHarris.JsonRpc;
using Rubberduck.InternalApi.RPC;
using Rubberduck.InternalApi.RPC.LSP.Response;
using Rubberduck.RPC.Platform;
using System.Threading.Tasks;
using WebSocketSharp;

namespace Rubberduck.Server.Controllers
{
    public class CodeLensController : JsonRpcClient
    {
        public CodeLensController(WebSocket socket) : base(socket)
        {
        }

        [JsonRpcMethod(JsonRpcMethods.ResolveCodeLens)]
        public async Task<CodeLens> Resolve(CodeLens parameters)
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.ResolveCodeLens, parameters);
                var response = Request<CodeLens>(request);

                return response;
            });
        }
    }
}
