namespace Rubberduck.RPC.Proxy.SharedServices.Abstract
{
    /// <summary>
    /// The base interface for a RSP/LSP server proxy.
    /// </summary>
    /// <typeparam name="TOptions">The class type for the configuration options of this server.</typeparam>
    /// <remarks>
    /// This interface must be implemented on the server side.
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