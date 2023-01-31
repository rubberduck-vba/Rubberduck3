using Rubberduck.RPC.Proxy.SharedServices.Server.Commands;

namespace Rubberduck.RPC.Platform
{
    /// <summary>
    /// An abstract factory that creates a ServerService implementation.
    /// </summary>
    /// <typeparam name="TService">The class type of the service to create.</typeparam>
    /// <typeparam name="TOptions">The class type of server configuration options.</typeparam>
    public interface IServerServiceFactory<TService, TOptions>
        where TService : ServerService<TOptions>
        where TOptions : class, new()
    {
        /// <summary>
        /// Creates a new instance of the ServerService.
        /// </summary>
        TService Create(ServerCommands<TOptions> commands);
    }
}
