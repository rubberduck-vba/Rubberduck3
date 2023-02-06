using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Platform.Model;
using Rubberduck.RPC.Proxy.SharedServices;
using Rubberduck.RPC.Proxy.SharedServices.Server.Commands;
using System;
using System.Threading.Tasks;

namespace Rubberduck.RPC.Proxy.LocalDbServer
{
    public interface ILocalDbServerProxyClient : IServerProxyClient<LocalDbServerCapabilities>
    {
        /// <summary>
        /// A notification sent from a client to the server to signal it is ready to send and receive requests and notifications.
        /// </summary>
        /// <remarks>
        /// The semantics are different than LSP here, the client does not own the server process.
        /// This notification sends a <c>processId</c> to identify the connecting client process.
        /// </remarks>
        event EventHandler<ClientInitializedParams> ClientInitialized;
        Task OnClientInitializedAsync(ClientInitializedParams parameter);

        /// <summary>
        /// A notification sent from a client to the server to signal it is shutting down and disconnecting from the server.
        /// </summary>
        event EventHandler<ClientShutdownParams> ClientShutdown;
        Task OnClientShutdownAsync(ClientShutdownParams parameter);

        /// <summary>
        /// Requests server information.
        /// </summary>
        /// <returns>The current server state.</returns>
        [RubberduckSP(JsonRpcMethods.ServerProxyRequests.Shared.Server.Info)]
        Task<ServerState> OnRequestServerInfoAsync();
    }
}
