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
    /// <typeparam name="TCommands">The class type of the class that exposes the commands for this service.</typeparam>
    public abstract class ServerProxyService<TOptions, TCommands, TProxyClient> : IConfigurableServerProxy<TOptions, TProxyClient, TCommands>, IJsonRpcTarget<TProxyClient>
        where TOptions : SharedServerCapabilities, new()
        where TCommands : class
        where TProxyClient : class, IJsonRpcSource
    {
        protected ServerProxyService(IServerLogger logger, IServerStateService<TOptions> serverStateService)
        {
            Logger = logger;
            ServerStateService = serverStateService ?? throw new ArgumentNullException(nameof(serverStateService));
        }

        public TOptions Configuration => ServerStateService.Configuration;

        public abstract TCommands Commands { get; }

        public virtual IServerLogger Logger { get; }

        public IServerStateService<TOptions> ServerStateService { get; }

        public TProxyClient ClientProxy { get; private set; }

        public void InitializeClientProxy(TProxyClient proxy)
        {
            if (!(ClientProxy is null))
            {
                throw new InvalidOperationException($"{nameof(ClientProxy)} was already initialized once.");
            }

            ClientProxy = proxy;
            if (!(ClientProxy is null))
            {
                RegisterClientNotifications(proxy);
            }
        }

        /// <summary>
        /// Registers events/notifications from the provided client proxy.
        /// </summary>
        /// <param name="client">The client to register notifications for.</param>
        protected abstract void RegisterClientNotifications(TProxyClient client);

        /// <summary>
        /// Removes event handler registrations for previously registered events of the provided client proxy.
        /// </summary>
        /// <param name="client">The client to deregister notifications for.</param>
        protected abstract void DeregisterClientNotifications(TProxyClient client);

        public void InitializeClientProxy(object proxy) => InitializeClientProxy(proxy as TProxyClient);
    }
}
