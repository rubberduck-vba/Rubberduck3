using Rubberduck.InternalApi.RPC.DataServer.Parameters;
using Rubberduck.InternalApi.RPC.DataServer.Response;
using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Proxy;
using WebSocketSharp;

namespace Rubberduck.Server
{
    internal class DataServerControllerProxy : JsonRpcClient, ILocalDbServerController
    {
        public DataServerControllerProxy(WebSocket socket) : base(socket)
        {
            
        }

        public ConnectResult Connect(ConnectParams parameters)
        {
            var request = CreateRequest("connect", parameters);
            return Request<ConnectResult>(request);
        }

        public DisconnectResult Disconnect(DisconnectParams parameters)
        {
            var request = CreateRequest("disconnect", parameters);
            return Request<DisconnectResult>(request);
        }

        public void Exit()
        {
            var request = CreateRequest("exit", null);
            Notify(request);
        }
    }
}
