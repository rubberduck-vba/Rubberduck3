using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Proxy.SharedServices.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Server.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public IEnumerable<IJsonRpcSource> ClientProxies { get; private set; }

        public void Initialize(IEnumerable<IJsonRpcSource> clientProxies)
        {
            if (ClientProxies != null)
            {
                throw new InvalidOperationException("This instance was already intiialized. As a proxy service, its instancing should be scoped.");
            }

            ClientProxies = clientProxies;
            if (ClientProxies?.Any() ?? false)
            {
                RegisterClientProxyNotifications(clientProxies);
            }
        }

        protected abstract void RegisterClientProxyNotifications(IEnumerable<IJsonRpcSource> clientProxies);
    }
}
