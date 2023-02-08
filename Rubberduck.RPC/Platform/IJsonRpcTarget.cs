using System;
using System.Collections.Generic;

namespace Rubberduck.RPC.Platform
{ 
    /// <summary>
    /// Represents a local RPC target, i.e. a RPC server-side proxy.
    /// </summary>
    public interface IJsonRpcTarget
    {
        void Initialize(IEnumerable<IJsonRpcSource> clientProxies);

        IEnumerable<IJsonRpcSource> ClientProxies { get; }
    }
}