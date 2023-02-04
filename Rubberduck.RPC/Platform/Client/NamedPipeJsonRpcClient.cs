using System;
using System.Threading.Tasks;
using System.IO.Pipes;
using System.Threading;

namespace Rubberduck.RPC.Platform.Client
{
    /// <summary>
    /// Represents a RPC client service that communicates over named pipes.
    /// </summary>
    public abstract class NamedPipeJsonRpcClient : JsonRpcClient<NamedPipeClientStream>
    {
        protected NamedPipeJsonRpcClient(IRpcStreamFactory<NamedPipeClientStream> rpcStreamFactory)
            : base(rpcStreamFactory)
        {
        }

        protected override async Task ConnectAsync(NamedPipeClientStream stream) // TODO configure connection timeout and cancellation token
            => await stream.ConnectAsync((int)TimeSpan.FromSeconds(5).TotalMilliseconds, CancellationToken.None);
    }
}