using AustinHarris.JsonRpc;
using Rubberduck.InternalApi.RPC;
using Rubberduck.InternalApi.RPC.LSP.Response;
using Rubberduck.RPC.Platform;
using System.Threading.Tasks;
using WebSocketSharp;

namespace Rubberduck.Server.Controllers
{
    public class CompletionItemController : JsonRpcClient
    {
        public CompletionItemController(WebSocket socket) : base(socket)
        {
        }

        [JsonRpcMethod(JsonRpcMethods.ResolveCompletionItem)]
        public async Task<CompletionItem> Resolve(CompletionItem parameters)
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.ResolveCompletionItem, parameters);
                var response = Request<CompletionItem>(request);

                return response;
            });
        }
    }
}
