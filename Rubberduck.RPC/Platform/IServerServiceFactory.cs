using Rubberduck.RPC.Proxy.SharedServices;
using Rubberduck.RPC.Proxy.SharedServices.Server.Commands;

namespace Rubberduck.RPC.Platform
{
    /// <summary>
    /// An abstract factory that creates a ServerService implementation.
    /// </summary>
    /// <typeparam name="TService">The class type of the service to create.</typeparam>
    /// <typeparam name="TOptions">The class type of server configuration options.</typeparam>
    /// <typeparam name="TClientProxy">A provider for server-level client-to-server notifications/events.</typeparam>
    public interface IServerServiceFactory<TService, TOptions, TClientProxy>
        where TService : ServerService<TOptions, TClientProxy>
        where TOptions : class, new()
        where TClientProxy : IServerProxyClient
    {
        /// <summary>
        /// Creates a new instance of the ServerService.
        /// </summary>
        TService Create(ServerCommands<TOptions> commands);
    }
}
