using System;
using System.Threading.Tasks;
using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.SharedServices.Console.Commands.Parameters;

namespace Rubberduck.RPC.Proxy.SharedServices.Console.Abstract
{
    public interface IServerConsoleProxyClient : IJsonRpcSource
    {
        /// <summary>
        /// A notification sent from the client to the server to modify the server's trace level.
        /// </summary>
        [LspCompliant(JsonRpcMethods.ServerProxyRequests.Shared.Console.SetTrace)]
        event EventHandler<SetTraceParams> SetTrace;
        Task OnSetTraceAsync(SetTraceParams parameter);
    }
}
