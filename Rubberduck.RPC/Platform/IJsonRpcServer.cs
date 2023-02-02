using Rubberduck.RPC.Platform.Model;
using Microsoft.Extensions.Hosting;

namespace Rubberduck.RPC.Platform
{
    /// <summary>
    /// Represents a JsonRPC server.
    /// </summary>
    public interface IJsonRpcServer : IHostedService
    {
        /// <summary>
        /// Gets information about this server instance.
        /// </summary>
        ServerState Info { get; }
    }
}
