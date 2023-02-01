using Rubberduck.RPC.Platform;
using Rubberduck.Server.LocalDb.Services;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.Server.LocalDb
{
    /// <summary>
    /// A <c>JsonRpcServer</c> that runs a local database and transports RPC messages over named pipe streams.
    /// </summary>
    /// <remarks>
    /// Holds the server state for the lifetime of the host process.
    /// </remarks>
    internal class LocalDbServer : JsonRpcServer<NamedPipeServerStream, LocalDbServerService, ServerCapabilities>
    {
        public LocalDbServer(IRpcStreamFactory<NamedPipeServerStream> rpcStreamFactory, IEnumerable<IJsonRpcTarget> proxies) 
            : base(rpcStreamFactory, proxies)
        {
        }

        protected override async Task WaitForConnectionAsync(NamedPipeServerStream stream, CancellationToken token) 
            => await stream.WaitForConnectionAsync(token);
    }
}
