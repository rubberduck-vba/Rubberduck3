using AustinHarris.JsonRpc;
using Rubberduck.InternalApi.RPC;
using Rubberduck.InternalApi.RPC.DataServer.Parameters;
using Rubberduck.InternalApi.RPC.DataServer.Response;
using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Proxy.Controllers;
using WebSocketSharp;

namespace Rubberduck.Server
{
    internal class LocalDbServerControllerProxy : JsonRpcClient, ILocalDbServerController
    {
        public LocalDbServerControllerProxy(WebSocket socket) : base(socket)
        {
            
        }

        [JsonRpcMethod("exit")]
        public void Exit()
        {
            var request = CreateRequest("exit", null);
            Notify(request);
        }

        public ConsoleStatusResult OptionsChanged()
        {
            throw new System.NotImplementedException();
        }

        public ConsoleStatusResult SetOptions(ServerConsoleOptions parameters)
        {
            throw new System.NotImplementedException();
        }

        public ConsoleStatusResult Status()
        {
            throw new System.NotImplementedException();
        }
    }
}
