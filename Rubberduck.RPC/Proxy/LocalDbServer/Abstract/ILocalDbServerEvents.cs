using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Platform.Model;
using System;

namespace Rubberduck.RPC.Proxy.LocalDbServer.Abstract
{
    public interface ILocalDbServerEvents
    {
        /// <summary>
        /// Signals a newly connected client to all other clients.
        /// </summary>
        [RubberduckSP("clientConnected")]
        event EventHandler<ServerState> ClientConnected;
        /// <summary>
        /// Signals a newly disconnected client to all other clients.
        /// </summary>
        [RubberduckSP("clientDisconnected")]
        event EventHandler<ServerState> ClientDisconnected;

        /// <summary>
        /// Signals to any still connected clients that the server process is about to terminate.
        /// </summary>
        [RubberduckSP("willExit")]
        event EventHandler WillExit;
    }
}
