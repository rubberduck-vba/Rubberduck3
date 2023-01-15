using NLog;
using Rubberduck.InternalApi.Common;
using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Proxy;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Rubberduck.Server.LocalDb
{
    /// <summary>
    /// A <c>JsonRpcServer</c> that runs a local database.
    /// </summary>
    public class LocalDbServer : JsonRpcServer, ILocalDbServerEvents, ILocalDbServer
    {
        public event EventHandler OnClientConnected;
        public event EventHandler OnClientDisconnected;

        public LocalDbServer(string jsonRpcServerPath, int port, IJsonRpcConsole console, bool interactive, TimeSpan exitDelay)
            : base(jsonRpcServerPath, port, console, interactive, exitDelay)
        {
        }

        private readonly ConcurrentDictionary<int, Client> _clients = new ConcurrentDictionary<int, Client>();
        public IEnumerable<Client> Clients => _clients.Values;

        public bool Connect(Client client)
        {
            Console.Log(LogLevel.Info, $"Connecting client '{client.Name}' (process ID: {client.ProcessId})...");

            if (!IsAlive)
            {
                Console.Log(LogLevel.Warn, $"Connection isn't alive; restarting server...");
                Start();
            }

            var isClientAdded = false;
            TimedAction.Run(() =>
            {
                isClientAdded = _clients.TryAdd(client.ProcessId, client);
                if (isClientAdded)
                {
                    OnClientConnected?.Invoke(this, EventArgs.Empty);
                    Console.Log(LogLevel.Info, $"Client '{client.Name}' (process ID: {client.ProcessId}) has connected successfully.");
                }
            });
            return isClientAdded;
        }

        public bool Disconnect(Client client)
        {
            Console.Log(LogLevel.Info, $"Disconnecting client '{client.Name}' (process ID: {client.ProcessId})...");

            var isClientRemoved = false;
            TimedAction.Run(() =>
            {
                isClientRemoved = _clients.TryRemove(client.ProcessId, out _);
                if (isClientRemoved)
                {
                    OnClientDisconnected?.Invoke(this, EventArgs.Empty);
                    Console.Log(LogLevel.Info, $"Client '{client.Name}' (process ID: {client.ProcessId}) has disconnected successfully.");

                    if (_clients.Count == 0 && IsAlive)
                    {
                        Console.Log(LogLevel.Warn, $"Last client has disconnected; server will be stopped.");
                        Stop();
                    }
                }
            });
            return isClientRemoved;
        }

        public bool HasClients => _clients.Any();
    }
}
