using AustinHarris.JsonRpc;
using Rubberduck.InternalApi.RPC;
using Rubberduck.InternalApi.RPC.LSP.Response;
using Rubberduck.RPC.Platform;
using System.Threading.Tasks;
using WebSocketSharp;

namespace Rubberduck.Server.Controllers
{
    public class DocumentLinkController : JsonRpcClient
    {
        public DocumentLinkController(WebSocket socket) : base(socket)
        {
        }

        [JsonRpcMethod(JsonRpcMethods.ResolveDocumentLink)]
        public async Task<DocumentLink> Resolve(DocumentLink parameters)
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.ResolveDocumentLink, parameters);
                var response = Request<DocumentLink>(request);

                return response;
            });
        }
    }
}
