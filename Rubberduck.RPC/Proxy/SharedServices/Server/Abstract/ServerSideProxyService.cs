using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Proxy.SharedServices.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Server.Configuration;
using System;

namespace Rubberduck.RPC.Proxy.SharedServices.Server.Abstract
{
    /// <summary>
    /// A common base class for all server proxy services that expose the server-side RPC targets.
    /// </summary>
    /// <typeparam name="TOptions">The class type of the configuration options for this service.</typeparam>
    public abstract class ServerSideProxyService<TOptions> : IConfigurableServerProxy<TOptions>, IJsonRpcTarget
        where TOptions : SharedServerCapabilities, new()
    {
        protected ServerSideProxyService(IServerLogger logger, IServerStateService<TOptions> serverStateService)
        {
            Logger = logger; // logger may be supplied by derived class
            ServerStateService = serverStateService ?? throw new ArgumentNullException(nameof(serverStateService));
        }

        public TOptions ServerOptions => ServerStateService.Configuration;

        public virtual IServerLogger Logger { get; }

        public IServerStateService<TOptions> ServerStateService { get; }
    }
}
