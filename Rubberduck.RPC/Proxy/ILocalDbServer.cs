using Rubberduck.InternalApi.RPC;
using Rubberduck.InternalApi.RPC.LSP.Parameters;
using Rubberduck.RPC.Parameters;
using Rubberduck.RPC.Platform;
using System.Collections.Generic;
using System.Threading.Tasks;

using DataServer = Rubberduck.InternalApi.RPC.DataServer.Capabilities;
using LspServer = Rubberduck.InternalApi.RPC.LSP.Capabilities;

namespace Rubberduck.RPC.Proxy
{
    public interface IJsonRpcServerProxy<TServerCapabilities> 
        where TServerCapabilities : class, new()
    {
        Task Disconnect(ClientInfo client);
        Task Exit();
        Task<InitializeResult<TServerCapabilities>> Initialize(LspInitializeParams parameters);
        Task Initialized(InitializedParams parameters);
        Task LogTrace(LogTraceParams parameters);
        Task SetTrace(SetTraceParams parameters);

        IJsonRpcConsole Console { get; }
    }

    public interface ILocalDbServer : IJsonRpcServerProxy<DataServer.ServerCapabilities>
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
        /// Sends a shutdown signal, terminating the host process.
        /// </summary>
        void Shutdown();
    }

    public interface ILSPServer : IJsonRpcServerProxy<LspServer.ServerCapabilities>
    {
        /* TODO */
    }
}