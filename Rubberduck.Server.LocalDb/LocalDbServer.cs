using NLog;
using Rubberduck.InternalApi.Common;
using Rubberduck.InternalApi.RPC;
using Rubberduck.InternalApi.RPC.DataServer.Capabilities;
using Rubberduck.InternalApi.RPC.LSP.Parameters;
using Rubberduck.RPC.Parameters;
using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Proxy;
using Rubberduck.Server.LocalDb.Properties;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rubberduck.Server.LocalDb
{
    /// <summary>
    /// A <c>JsonRpcServer</c> that runs a local database.
    /// </summary>
    public class LocalDbServer : JsonRpcServer, ILocalDbServerEvents, ILocalDbServer
    {
        /// <summary>
        /// Signals the server shutting down, to terminate the host process.
        /// </summary>
        public event EventHandler ShutdownSignal;
        public event EventHandler ClientConnected;
        public event EventHandler ClientDisconnected;

        public LocalDbServer(string jsonRpcServerPath, int port, IJsonRpcConsole console)
            : base(jsonRpcServerPath, port, console)
        {
        }

        private void OnShutdownSignal()
        {
            ShutdownSignal?.Invoke(this, EventArgs.Empty);
        }

        private void OnClientConnected()
        {
            ClientConnected?.Invoke(this, EventArgs.Empty);
        }

        private void OnClientDisconnected()
        {
            ClientDisconnected?.Invoke(_clients, EventArgs.Empty);
        }

        private readonly ConcurrentDictionary<int, Client> _clients = new ConcurrentDictionary<int, Client>();
        public IEnumerable<Client> Clients => _clients.Values;

        public async Task Disconnect(ClientInfo client)
        {
            OnClientDisconnected();
        }

        public async Task Exit()
        {
            OnShutdownSignal();
        }

        public async Task<InitializeResult<ServerCapabilities>> Initialize(LspInitializeParams parameters)
        {
            OnClientConnected();
            return new InitializeResult<ServerCapabilities>
            {
                ServerInfo = new InitializeResult<ServerCapabilities>.ServerInformation
                {
                    Name = Path,
                    ProcessId = ProcessId,
                    StartTimestamp = SessionStart.Value,
                    Version = "0.1",
                },
                Capabilities = new ServerCapabilities
                {
                    // TODO
                },
            };
        }

        public async Task Initialized(InitializedParams parameters)
        {
            // no-op for dbserver
        }

        public Task LogTrace(LogTraceParams parameters)
        {
            throw new NotImplementedException();
        }

        public Task SetTrace(SetTraceParams parameters)
        {
            throw new NotImplementedException();
        }

        public bool HasClients => _clients.Any();

        public void Shutdown()
        {
            Exit().Wait();
        }
    }
}
