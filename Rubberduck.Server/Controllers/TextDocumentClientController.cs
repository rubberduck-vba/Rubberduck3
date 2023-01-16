using Rubberduck.InternalApi.RPC.LSP.Parameters;
using System.Threading.Tasks;
using Rubberduck.RPC.Platform;
using WebSocketSharp;
using Rubberduck.InternalApi.RPC;

namespace Rubberduck.Server.Controllers
{
    public class TextDocumentClientController : JsonRpcClient
    {
        public TextDocumentClientController(WebSocket socket) : base(socket)
        {
        }

        public async Task PublishDiagnostics(PublishDiagnosticsParams parameters)
        {
            await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.PublishTextDocumentDiagnostics, parameters);
                Notify(request);
            });
        }
    }
}
