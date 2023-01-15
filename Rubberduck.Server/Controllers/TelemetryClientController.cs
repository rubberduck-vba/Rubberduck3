using Rubberduck.InternalApi.RPC.LSP;
using Rubberduck.RPC.Platform;
using System.Threading.Tasks;
using WebSocketSharp;

namespace Rubberduck.Server.Controllers
{
    public class TelemetryClientController : JsonRpcClient
    {
        public TelemetryClientController(WebSocket socket) : base(socket)
        {
        }

        public async Task TelemetryEvent(object parameters)
        {
            await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.TelemetryEvent, parameters);
                Notify(request);
            });
        }
    }
}
