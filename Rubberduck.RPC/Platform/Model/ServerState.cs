using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;

namespace Rubberduck.RPC.Platform.Model
{
    /// <summary>
    /// Holds the immutable base server state.
    /// </summary>
    public interface IServerInfo
    {
        /// <summary>
        /// The name of the server.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// The ID of the host process.
        /// </summary>
        int ProcessId { get; }
        /// <summary>
        /// The start date/time of this server. <c>null</c> if the server wasn't started.
        /// </summary>
        DateTime? StartTime { get; }
        /// <summary>
        /// The server application version.
        /// </summary>
        string Version { get; }
    }

    /// <summary>
    /// Holds the immutable base server state.
    /// </summary>
    /// <remarks>
    /// Implementation exposes setters to facilitate serialization.
    /// </remarks>
    public class BasicServerInfo : IServerInfo
    {
        public int ProcessId { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public DateTime? StartTime { get; set; }
    }

    /// <summary>
    /// Represents the server's internal mutable state.
    /// </summary>
    public interface IServerState : IServerInfo
    {
        /// <summary>
        /// The clients connected to this server.
        /// </summary>
        RpcClientInfo[] Clients { get; }
        /// <summary>
        /// Whether the JsonRpcServer is alive and accepts client connections.
        /// </summary>
        bool IsAlive { get; }
        /// <summary>
        /// The number of messages received by this server.
        /// </summary>
        int MessagesReceived { get; }
        /// <summary>
        /// The number of messages sent by this server.
        /// </summary>
        int MessagesSent { get; }
        /// <summary>
        /// Peak working set memory, in bytes.
        /// </summary>
        long PeakWorkingSet { get; }
        /// <summary>
        /// The current status state of this server.
        /// </summary>
        ServerStatus Status { get; }
        /// <summary>
        /// The number of threads in the host process.
        /// </summary>
        int Threads { get; }
        /// <summary>
        /// Working set memory, in bytes.
        /// </summary>
        long WorkingSet { get; }

        long GC { get; }
    }

    /// <summary>
    /// Represents the server's internal mutable state.
    /// </summary>
    public class ServerState : BasicServerInfo, IServerState
    {
        private readonly ConcurrentDictionary<string, (RpcClientInfo Client, bool Initialized)> _clients = new ConcurrentDictionary<string, (RpcClientInfo, bool)>();

        private int _received = 0;
        private int _sent = 0;

        public ServerState() { }

        public ServerState(IServerState info)
            : this((IServerInfo)info)
        {
            WorkingSet = info.WorkingSet;
            PeakWorkingSet = info.PeakWorkingSet;
            Threads = info.Threads;
            MessagesSent = info.MessagesSent;
            MessagesReceived = info.MessagesReceived;
            IsAlive = info.IsAlive;
            Status = info.Status;

            Clients = info.Clients;
            GC = info.GC;
        }

        public ServerState(IServerInfo info)
        {
            Name = info.Name;
            Version = info.Version;
            ProcessId = info.ProcessId;
            StartTime = info.StartTime;
        }

        public void OnMessageReceived() => Interlocked.Increment(ref _received);
        public void OnMessageSent() => Interlocked.Increment(ref _sent);

        /// <summary>
        /// Adds the specified client to the server state.
        /// </summary>
        /// <param name="client">The client to be added.</param>
        /// <returns><c>true<c/> if the client was successfully added.</returns>
        public bool Connect(RpcClientInfo client)
        {
            var didConnect = false;

            if (_clients.TryAdd(client.Name, (client, Initialized: false)))
            {
                didConnect = true;
                UpdateClients();
            }

            return didConnect && _clients.ContainsKey(client.Name);
        }

        /// <summary>
        /// Removes from the server state the client associated to the specified client.
        /// </summary>
        /// <returns><c>true</c> if the client was successfully removed.</returns>
        public bool Disconnect(string name, out RpcClientInfo client)
        {
            client = null;
            var didRemove = false;

            if (_clients.TryRemove(name, out var removed))
            {
                client = removed.Client;
                didRemove = true;
                UpdateClients();
            }

            return didRemove && !_clients.ContainsKey(name);
        }

        private void UpdateClients()
        {
            Clients = _clients.Select(e => e.Value.Client).ToArray();
        }

        /// <summary>
        /// Sets the initialized state of the specified client.
        /// </summary>
        public void SetInitialized(string name)
        {
            var clientState = _clients[name];
            clientState.Initialized = true;

            _clients[name] = clientState;
        }

        [JsonPropertyName("memory")]
        public long WorkingSet { get; set; }

        [JsonPropertyName("peakMemory")]
        public long PeakWorkingSet { get; set; }

        [JsonPropertyName("threads")]
        public int Threads { get; set; }

        [JsonPropertyName("clients")]
        public RpcClientInfo[] Clients { get; set; }

        [JsonPropertyName("sent")]
        public int MessagesSent { get => _sent; set => _sent = value; }

        [JsonPropertyName("received")]
        public int MessagesReceived { get => _received; set => _received = value; }

        [JsonPropertyName("isAlive")]
        public bool IsAlive { get; set; }

        [JsonPropertyName("uptime")]
        public TimeSpan? UpTime { get; set; }

        [JsonPropertyName("state")]
        public ServerStatus Status { get; set; }

        [JsonPropertyName("gc")]
        public long GC { get; set; }

        public int ClientsCount => Clients.Length;
    }
}
