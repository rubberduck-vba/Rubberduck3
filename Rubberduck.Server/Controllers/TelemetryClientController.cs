using Rubberduck.InternalApi.RPC;
using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Telemetry;
using System.Threading.Tasks;
using WebSocketSharp;

namespace Rubberduck.Server.Controllers
{
    public class TelemetryClientController : JsonRpcClient
    {
        public TelemetryClientController(WebSocket socket) : base(socket)
        {
        }

        public async Task TelemetryEvent(TelemetryEvent parameters)
        {
            await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.TelemetryEvent, parameters);
                Notify(request);
            });
        }
    }
}
