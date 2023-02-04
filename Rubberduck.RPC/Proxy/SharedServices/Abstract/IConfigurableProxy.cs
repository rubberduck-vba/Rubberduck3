namespace Rubberduck.RPC.Proxy.SharedServices.Abstract
{
    /// <summary>
    /// The base interface for a configurable RPC proxy (client or server).
    /// </summary>
    /// <typeparam name="TOptions">The class type for the configuration options for this proxy.</typeparam>
    /// <remarks>
    /// Proxy implementations should be stateless: the instance only lives for the duration of a single request.
    /// </remarks>
    public interface IConfigurableProxy<TOptions>
        where TOptions : class, new()
    {
        /// <summary>
        /// Configuration options for this server.
        /// </summary>
        TOptions Configuration { get; }
    }
}