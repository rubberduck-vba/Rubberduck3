using Rubberduck.RPC.Proxy.SharedServices;
using Rubberduck.RPC.Proxy.SharedServices.Server.Commands;
using System;

namespace Rubberduck.RPC.Proxy.LocalDbServer.Abstract
{
    public interface ILocalDbServerProxyClient : IServerProxyClient
    {
        /// <summary>
        /// A notification sent from a client to the server to signal it is ready to send and receive requests and notifications.
        /// </summary>
        /// <remarks>
        /// The semantics are different than LSP here, the client does not own the server process.
        /// This notification sends a <c>processId</c> to identify the connecting client process.
        /// </remarks>
        event EventHandler<ClientInitializedParams> ClientInitialized;
        /// <summary>
        /// A notification sent from a client to the server to signal it is shutting down and disconnecting from the server.
        /// </summary>
        event EventHandler<ClientShutdownParams> ClientShutdown;
    }
}
