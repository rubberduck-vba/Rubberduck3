using Rubberduck.RPC.Proxy.SharedServices;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.Json.Serialization;

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
        [JsonPropertyName("processId")]
        public int ProcessId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("startTime")]
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
        ClientInfo[] Clients { get; }
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
        /// Time elapsed since this server was started. <c>null</c> if the server wasn't started.
        /// </summary>
        TimeSpan? UpTime { get; }
        /// <summary>
        /// Working set memory, in bytes.
        /// </summary>
        long WorkingSet { get; }
    }

    /// <summary>
    /// Represents the server's internal mutable state.
    /// </summary>
    public class ServerState : BasicServerInfo, IServerState
    {
        private readonly ConcurrentDictionary<int, (ClientInfo Client, bool Initialized)> _clientsByProcessId = new ConcurrentDictionary<int, (ClientInfo, bool)>();

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
            UpTime = info.UpTime;
            Status = info.Status;

            Clients = info.Clients;
        }

        public ServerState(IServerInfo info)
        {
            Name = info.Name;
            Version = info.Version;
            ProcessId = info.ProcessId;
            StartTime = info.StartTime;
        }

        /// <summary>
        /// Adds the specified client to the server state.
        /// </summary>
        /// <param name="client">The client to be added.</param>
        /// <returns><c>true<c/> if the client was successfully added.</returns>
        public bool Connect(ClientInfo client)
        {
            var didConnect = false;

            if (_clientsByProcessId.TryAdd(client.ProcessId, (client, Initialized: false)))
            {
                didConnect = true;
                UpdateClients();
            }

            return didConnect && _clientsByProcessId.ContainsKey(client.ProcessId);
        }

        /// <summary>
        /// Removes from the server state the client associated to the specified process ID.
        /// </summary>
        /// <param name="processId">The process ID of the client to disconnect.</param>
        /// <param name="client">The disconnected client, if it was successfully disconnected.</param>
        /// <returns><c>true</c> if the client was successfully removed.</returns>
        public bool Disconnect(int processId, out ClientInfo client)
        {
            client = null;
            var didRemove = false;

            if (_clientsByProcessId.TryRemove(processId, out var removed))
            {
                client = removed.Client;
                didRemove = true;
                UpdateClients();
            }

            return didRemove && !_clientsByProcessId.ContainsKey(processId);
        }

        private void UpdateClients()
        {
            Clients = _clientsByProcessId.Select(e => e.Value.Client).ToArray();
        }

        /// <summary>
        /// Sets the initialized state of the specified client process ID.
        /// </summary>
        /// <param name="processId">The process ID of the initialized client.</param>
        public void SetInitialized(int processId)
        {
            var clientState = _clientsByProcessId[processId];
            clientState.Initialized = true;

            _clientsByProcessId[processId] = clientState;
        }

        [JsonPropertyName("memory")]
        public long WorkingSet { get; set; }

        [JsonPropertyName("peakMemory")]
        public long PeakWorkingSet { get; set; }

        [JsonPropertyName("threads")]
        public int Threads { get; set; }

        [JsonPropertyName("clients")]
        public ClientInfo[] Clients { get; set; }

        [JsonPropertyName("sent")]
        public int MessagesSent { get; set; }

        [JsonPropertyName("received")]
        public int MessagesReceived { get; set; }

        [JsonPropertyName("isAlive")]
        public bool IsAlive { get; set; }

        [JsonPropertyName("uptime")]
        public TimeSpan? UpTime { get; set; }

        [JsonPropertyName("state")]
        public ServerStatus Status { get; set; }

        public int ClientsCount => Clients.Length;
    }
}
