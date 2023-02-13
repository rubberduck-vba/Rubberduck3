using StreamJsonRpc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rubberduck.RPC.Platform
{ 
    /// <summary>
    /// Represents a local RPC target, i.e. a RPC server-side proxy.
    /// </summary>
    public interface IJsonRpcTarget
    {
        [JsonRpcIgnore]
        void Initialize(IEnumerable<IJsonRpcSource> clientProxies);

        [JsonRpcIgnore]
        Task<IEnumerable<IJsonRpcSource>> GetClientProxies();
    }
}