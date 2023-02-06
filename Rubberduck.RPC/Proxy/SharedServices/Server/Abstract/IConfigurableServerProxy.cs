using Rubberduck.RPC.Proxy.SharedServices.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Server.Configuration;

namespace Rubberduck.RPC.Proxy.SharedServices.Server.Abstract
{
    /// <summary>
    /// The base interface for a RSP/LSP server proxy.
    /// </summary>
    /// <typeparam name="TOptions">The class type for the configuration options of this server.</typeparam>
    /// <remarks>
    /// This interface must be implemented on the server side.
    /// Proxy implementations should be stateless: the instance only lives for the duration of a single request.
    /// </remarks>
    public interface IConfigurableServerProxy<TOptions> : IConfigurableProxy<TOptions>
        where TOptions : SharedServerCapabilities, new()
    {
        /// <summary>
        /// Gets a service that returns the current server state.
        /// </summary>
        IServerStateService<TOptions> ServerStateService { get; }

        /// <summary>
        /// Gets the server's console logger.
        /// </summary>
        IServerLogger Logger { get; }
    }
}