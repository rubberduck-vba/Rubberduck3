using AustinHarris.JsonRpc;
using Rubberduck.InternalApi.RPC;
using Rubberduck.InternalApi.RPC.LSP.Response;
using Rubberduck.RPC.Platform;
using System.Threading.Tasks;
using WebSocketSharp;

namespace Rubberduck.Server.Controllers
{
    public class CodeActionController : JsonRpcClient
    {
        public CodeActionController(WebSocket socket) : base(socket)
        {
        }

        /// <summary>
        /// Resolves the edit and/or command associated to the specified code action.
        /// </summary>
        [JsonRpcMethod(JsonRpcMethods.ResolveCodeAction)]
        public async Task<CodeAction> Resolve(CodeAction parameters)
        {
            return await Task.Run(() => 
            {
                var request = CreateRequest(JsonRpcMethods.ResolveCodeAction, parameters);
                var response = Request<CodeAction>(request);

                return response;
            });
        }
    }
}
