using OmniSharp.Extensions.JsonRpc;
using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Model;
using Rubberduck.RPC.Platform.Model.Database.Responses;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubberduck.Server.RPC
{
    public interface ILocalDbServerServiceProxy
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
}
