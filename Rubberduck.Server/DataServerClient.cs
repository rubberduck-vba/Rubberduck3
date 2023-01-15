using AustinHarris.JsonRpc;
using Rubberduck.InternalApi.RPC.DataServer.Parameters;
using Rubberduck.InternalApi.RPC.DataServer.Response;
using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Proxy.Controllers;
using WebSocketSharp;

namespace Rubberduck.Server
{
    internal class DataServerControllerProxy : JsonRpcClient, ILocalDbServerController
    {
        public DataServerControllerProxy(WebSocket socket) : base(socket)
        {
            
        }

        [JsonRpcMethod("connect")]
        public ConnectResult Connect(ConnectParams parameters)
        {
            var request = CreateRequest("connect", parameters);
            return Request<ConnectResult>(request);
        }

        [JsonRpcMethod("disconnect")]
        public DisconnectResult Disconnect(DisconnectParams parameters)
        {
            var request = CreateRequest("disconnect", parameters);
            return Request<DisconnectResult>(request);
        }

        [JsonRpcMethod("exit")]
        public void Exit()
        {
            var request = CreateRequest("exit", null);
            Notify(request);
        }
    }
}
