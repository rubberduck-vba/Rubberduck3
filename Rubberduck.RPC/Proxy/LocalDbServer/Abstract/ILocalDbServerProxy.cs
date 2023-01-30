using AustinHarris.JsonRpc;
using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.LspServer.Configuration.Capabilities;
using Rubberduck.RPC.Proxy.SharedServices.Server.Abstract;

namespace Rubberduck.RPC.Proxy.LocalDbServer.Abstract
{
    public interface ILocalDbServerProxy : IServerProxyService<ServerCapabilities>, ILocalDbServerEvents
    {
        /// <summary>
        /// Sends a shutdown signal, asking the server to shut down.
        /// </summary>
        /// <remarks>
        /// Client should not send any request or notification other than <see cref="RequestExit"/> afterwards.
        /// </remarks>
        [JsonRpcMethod(JsonRpcMethods.ServerProxyRequests.Shared.Server.Shutdown), LspCompliant]
        void RequestShutdown();
    }
}
