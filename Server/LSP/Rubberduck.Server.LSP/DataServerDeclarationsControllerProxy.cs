using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Proxy.Controllers;
using WebSocketSharp;

namespace Rubberduck.Server
{
    internal class DataServerDeclarationsControllerProxy : JsonRpcClient, ILocalDbDeclarationsController
    {
        public DataServerDeclarationsControllerProxy(WebSocket socket) : base(socket)
        {
        }

        /* TODO */
    }
}
