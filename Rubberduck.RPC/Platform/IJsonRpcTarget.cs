using System;

namespace Rubberduck.RPC.Platform
{
    /// <summary>
    /// Represents a local RPC target, i.e. a RPC server-side proxy.
    /// </summary>
    public interface IJsonRpcTarget<TProxyClient> : IJsonRpcTarget
        where TProxyClient : IJsonRpcSource
    {
        /// <summary>
        /// Gets the client proxy.
        /// </summary>
        /// <remarks>
        /// <c>null</c> until initialized.
        /// </remarks>
        TProxyClient ClientProxy { get; }

        /// <summary>
        /// Injects the <c>ClientProxy</c> and registers its notifications.
        /// </summary>
        /// <param name="proxy">The client proxy to initialize.</param>
        void InitializeClientProxy(TProxyClient proxy);
    }

    /// <summary>
    /// Represents a local RPC target, i.e. a RPC server-side proxy.
    /// </summary>
    public interface IJsonRpcTarget
    {
        /// <summary>
        /// Injects a client proxy and registers its notifications.
        /// </summary>
        /// <remarks>
        /// Use on the server side.
        /// </remarks>
        /// <param name="proxy">The client proxy to initialize.</param>
        void InitializeClientProxy(object proxy);
    }
}