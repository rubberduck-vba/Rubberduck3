using Rubberduck.RPC.Platform.Model;
using Rubberduck.RPC.Proxy.SharedServices.Server.Configuration;

namespace Rubberduck.RPC.Proxy.SharedServices.Abstract
{
    public interface IServerStateService<TOptions> 
        where TOptions : SharedServerCapabilities, new()
    {
        /// <summary>
        /// Gets the server's mutable state.
        /// </summary>
        ServerState Info { get; }

        /// <summary>
        /// Gets the current server status.
        /// </summary>
        ServerStatus ServerStatus { get; }

        /// <summary>
        /// Gets the current server configuration.
        /// </summary>
        TOptions Configuration { get; }
    }
}
