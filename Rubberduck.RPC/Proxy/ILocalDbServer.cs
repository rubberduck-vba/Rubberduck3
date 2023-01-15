using System;
using System.Collections.Generic;

namespace Rubberduck.RPC.Proxy
{
    public interface ILocalDbServer
    {
        /// <summary>
        /// <c>true</c> if the server has one or more connected clients.
        /// </summary>
        bool HasClients { get; }
        /// <summary>
        /// The client processes connected to this server.
        /// </summary>
        IEnumerable<Client> Clients { get; }

        /// <summary>
        /// Connects a client to this server.
        /// </summary>
        /// <param name="client"></param>
        /// <returns><c>true</c> if the connection succeeded.</returns>
        bool Connect(Client client);

        /// <summary>
        /// Disconnects a client from this server.
        /// </summary>
        /// <param name="client"></param>
        /// <returns><c>true</c> if the client was successfully disconnected.</returns>
        bool Disconnect(Client client);
    }
}