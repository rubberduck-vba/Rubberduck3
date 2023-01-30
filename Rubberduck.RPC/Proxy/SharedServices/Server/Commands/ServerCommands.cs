using Rubberduck.RPC.Platform.Model;
using Rubberduck.RPC.Proxy.SharedServices.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Server.Model;
using Rubberduck.RPC.Proxy.SharedServices.Server.Responses;

namespace Rubberduck.RPC.Proxy.SharedServices.Server.Commands
{
    public class ServerCommands<TServerOptions>
        where TServerOptions : class, new()
    {
        public ServerCommands() { }

        /// <summary>
        /// A delegate that returns the current state of the server.
        /// </summary>
        public IServerRequestCommand<ServerState> ServerInfoCommand { get; internal set; }
        /// <summary>
        /// A delegate that adds a client to the server state.
        /// </summary>
        public IServerRequestCommand<InitializeParams<TServerOptions>, InitializeResult<TServerOptions>> InitializeCommand { get; internal set; }
        /// <summary>
        /// A delegate that sets the configuration options for the server.
        /// </summary>
        public IServerNotificationCommand<TServerOptions> SetConfigurationOptionsCommand { get; internal set; }

        /// <summary>
        /// A delegate that sets a client to an <c>Initialized</c> state.
        /// </summary>
        public IServerRequestCommand<ClientInfo, ConnectResult> ConnectClientCommand { get; internal set; }
        /// <summary>
        /// A delegate that removes a client process from the server state.
        /// </summary>
        public IServerRequestCommand<ClientShutdownParams, DisconnectResult> DisconnectClientCommand { get; internal set; }

        /// <summary>
        /// A command that terminates the host process.
        /// </summary>
        /// <remarks>
        /// Exit code depends on server state and the number of connected clients.
        /// </remarks>
        public IServerNotificationCommand ExitCommand { get; internal set; }
    }
}
