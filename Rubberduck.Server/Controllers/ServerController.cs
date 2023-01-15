using AustinHarris.JsonRpc;
using Rubberduck.InternalApi.RPC.LSP;
using Rubberduck.InternalApi.RPC.LSP.Parameters;
using Rubberduck.InternalApi.RPC.LSP.Response;
using Rubberduck.RPC.Platform;
using System.Threading.Tasks;
using WebSocketSharp;

namespace Rubberduck.Server.Controllers
{
    public class ServerController : JsonRpcClient
    {
        public ServerController(WebSocket socket) : base(socket)
        {
        }

        [JsonRpcMethod(JsonRpcMethods.Initialize)]
        public async Task<InitializeResult> Initialize(InitializeParams parameters)
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.Initialize, parameters);
                var response = Request<InitializeResult>(request);

                return response;
            });
        }

        [JsonRpcMethod(JsonRpcMethods.Initialized)]
        public async Task Initialized(InitializedParams parameters)
        {
            await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.Initialized, parameters);
                Notify(request);
            });
        }

        [JsonRpcMethod(JsonRpcMethods.Shutdown)]
        public async Task Shutdown()
        {
            await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.Shutdown, null);
                Notify(request);
            });
        }

        [JsonRpcMethod(JsonRpcMethods.Exit)]
        public async Task Exit()
        {
            await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.Exit, null);
                Notify(request);
            });
        }

        [JsonRpcMethod(JsonRpcMethods.SetTrace)]
        public async Task SetTrace(SetTraceParams parameters)
        {
            await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.SetTrace, parameters);
                Notify(request);
            });
        }

        [JsonRpcMethod(JsonRpcMethods.LogTrace)]
        public async Task LogTrace(LogTraceParams parameters)
        {
            await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.LogTrace, parameters);
                Notify(request);
            });
        }
    }
}
