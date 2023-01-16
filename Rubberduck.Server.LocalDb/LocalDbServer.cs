using NLog;
using Rubberduck.InternalApi.Common;
using Rubberduck.InternalApi.RPC;
using Rubberduck.InternalApi.RPC.DataServer.Capabilities;
using Rubberduck.InternalApi.RPC.LSP.Parameters;
using Rubberduck.RPC.Parameters;
using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Proxy;
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
        public event EventHandler OnClientConnected;
        public event EventHandler OnClientDisconnected;

        public LocalDbServer(string jsonRpcServerPath, int port, IJsonRpcConsole console, bool interactive, TimeSpan exitDelay)
            : base(jsonRpcServerPath, port, console, interactive, exitDelay)
        {
        }

        private readonly ConcurrentDictionary<int, Client> _clients = new ConcurrentDictionary<int, Client>();
        public IEnumerable<Client> Clients => _clients.Values;

        public Task Exit()
        {
            throw new NotImplementedException();
        }

        public Task<InitializeResult<ServerCapabilities>> Initialize(LspInitializeParams parameters)
        {
            throw new NotImplementedException();
        }

        public Task Initialized(InitializedParams parameters)
        {
            throw new NotImplementedException();
        }

        public Task LogTrace(LogTraceParams parameters)
        {
            throw new NotImplementedException();
        }

        public Task SetTrace(SetTraceParams parameters)
        {
            throw new NotImplementedException();
        }

        public Task Shutdown()
        {
            throw new NotImplementedException();
        }

        public bool HasClients => _clients.Any();
    }
}
