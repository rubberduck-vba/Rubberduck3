using System;

namespace Rubberduck.RPC.Platform
{
    /// <summary>
    /// A marker interface for a type to register as a JsonRpc target.
    /// </summary>
    /// <remarks>
    /// Might turn into an attribute.
    /// </remarks>
    public interface IJsonRpcTarget
    {
        /// <summary>
        /// Represents the client-side RPC proxy.
        /// </summary>
        /// <remarks>
        /// Fires events for client-to-server notifications. Invoked methods run client-side.
        /// </remarks>
        Type ClientProxyType { get; }
        void SetClientProxy<T>(T proxy) where T : class;
    }
}