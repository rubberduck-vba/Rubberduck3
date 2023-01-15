using Rubberduck.InternalApi.RPC.DataServer.Parameters;
using Rubberduck.InternalApi.RPC.DataServer.Response;

namespace Rubberduck.RPC.Proxy.Controllers
{
    public interface ILocalDbServerController
    {
        /// <summary>
        /// Connects a client to this server.
        /// </summary>
        ConnectResult Connect(ConnectParams parameters);

        /// <summary>
        /// Disconnects a client from this server.
        /// </summary>
        /// <remarks>
        /// If the last client disconnects, response will prompt the client to send a final <c>Exit</c> noticication.
        /// </remarks>
        DisconnectResult Disconnect(DisconnectParams parameters);

        /// <summary>
        /// Terminates the server process.
        /// </summary>
        /// <remarks>
        /// Only exits with code 0 after a <c>DisconnectResult</c> response was returned with <c>ShuttingDown</c> set to <c>true</c>.
        /// </remarks>
        void Exit();
    }
}
