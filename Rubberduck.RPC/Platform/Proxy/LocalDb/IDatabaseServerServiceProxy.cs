using OmniSharp.Extensions.JsonRpc;
using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Model;
using Rubberduck.RPC.Platform.Model.Database.Responses;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.Server.RPC
{
    public interface IDatabaseServerServiceProxy
    {
        [Method(JsonRpcMethods.Database.Connect, Direction.Bidirectional)]
        Task<SuccessResult> ConnectAsync(RpcClientInfo clientInfo);
        [Method(JsonRpcMethods.Database.Disconnect, Direction.Bidirectional)]
        Task<SuccessResult> DisconnectAsync(RpcClientInfo clientInfo);

        [Method(JsonRpcMethods.Database.Info, Direction.Bidirectional)]
        Task<InfoResult> GetServerInfoAsync();

        [Method(JsonRpcMethods.Database.SetTrace, Direction.ClientToServer)]
        Task SetTraceAsync();
    }

    public class DatabaseServerService : IDatabaseServerServiceProxy
    {
        private readonly IJsonRpcServerFacade _server;

        public DatabaseServerService(IJsonRpcServerFacade server)
        {
            _server = server;
        }

        public async Task<SuccessResult> ConnectAsync(RpcClientInfo clientInfo)
        {
            return await _server
                .SendRequest(JsonRpcMethods.Database.Connect, clientInfo)
                .Returning<SuccessResult>(CancellationToken.None);
        }

        public async Task<SuccessResult> DisconnectAsync(RpcClientInfo clientInfo)
        {
            return await _server
                .SendRequest(JsonRpcMethods.Database.Disconnect, clientInfo)
                .Returning<SuccessResult>(CancellationToken.None);
        }

        public async Task<InfoResult> GetServerInfoAsync()
        {
            return await _server
                .SendRequest(JsonRpcMethods.Database.Info)
                .Returning<InfoResult>(CancellationToken.None);
        }

        public async Task SetTraceAsync()
        {
            await _server
                .SendRequest(JsonRpcMethods.Database.SetTrace)
                .ReturningVoid(CancellationToken.None);
        }
    }
}
