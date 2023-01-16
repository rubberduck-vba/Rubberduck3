using Rubberduck.InternalApi.RPC;
using Rubberduck.InternalApi.RPC.DataServer.Capabilities;
using Rubberduck.InternalApi.RPC.LSP.Parameters;
using Rubberduck.RPC;
using Rubberduck.RPC.Parameters;
using Rubberduck.RPC.Platform;
using System;
using System.Threading.Tasks;
using WebSocketSharp;

namespace Rubberduck.Server.LocalDb.Controllers
{
    public class ServerController : JsonRpcClient, IServerController<ServerCapabilities>
    {
        public ServerController(WebSocket socket) : base(socket)
        {
        }

        public async Task Exit()
        {
            await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.Exit, null);
                Notify(request);
            });
        }

        public Task<InitializeResult<ServerCapabilities>> Initialize(LspInitializeParams parameters)
        {
            throw new NotImplementedException();
        }

        public Task Initialized(InitializedParams parameters)
        {
            throw new NotImplementedException();
        }

        public Task LogTrace(LogTraceParams parameters)
        {
            throw new NotImplementedException();
        }

        public Task SetTrace(SetTraceParams parameters)
        {
            throw new NotImplementedException();
        }

        public Task Shutdown()
        {
            throw new NotImplementedException();
        }
    }
}
