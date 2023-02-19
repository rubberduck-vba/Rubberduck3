using OmniSharp.Extensions.JsonRpc;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.Server.LocalDb
{

    internal class LocalDbServer
    {
        private readonly IJsonRpcServer _server;

        public LocalDbServer(IJsonRpcServer server)
        {
            _server = server;
        }
    }

    /*
    /// <summary>
    /// A <c>JsonRpcServer</c> that runs a local database and transports RPC messages over named pipe streams.
    /// </summary>
    /// <remarks>
    /// Holds the server state for the lifetime of the host process.
    /// </remarks>
    internal class LocalDbServer : NamedPipeJsonRpcServer<LocalDbServerProxyService, LocalDbServerCapabilities, InitializeParams<LocalDbServerCapabilities>>
    {
        public LocalDbServer(IServiceProvider serviceProvider, IEnumerable<Type> clientProxyTypes) 
            : base(serviceProvider, clientProxyTypes) 
        {
        }

        public GetServerStateInfoAsync GetServerState { get; internal set; }
        public override ServerState Info => GetServerState?.Invoke().ConfigureAwait(false).GetAwaiter().GetResult();

        protected override async Task WaitForConnectionAsync(NamedPipeServerStream stream, CancellationToken token) 
            => await stream.WaitForConnectionAsync(token);
    }
    */
}
