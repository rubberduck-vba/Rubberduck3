using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubberduck.DataServer
{
    internal class Server
    {
        public int ProcessId { get; }

        private readonly ConcurrentDictionary<int, Client> _clients = new ConcurrentDictionary<int, Client>();
        public IEnumerable<Client> Clients => _clients.Values;

        public bool Connect(Client client)
        {
            return _clients.TryAdd(client.ProcessId, client);
        }

        public bool Disconnect(Client client)
        {
            return _clients.TryRemove(client.ProcessId, out _);
        }

        public bool HasClients => _clients.Any();
    }

    internal class Client
    {
        public int ProcessId { get; set; }
        public string Name { get; set; }
    }
}
