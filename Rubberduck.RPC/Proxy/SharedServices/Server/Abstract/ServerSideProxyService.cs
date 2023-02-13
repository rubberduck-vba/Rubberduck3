using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Proxy.SharedServices.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Server.Configuration;
using StreamJsonRpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            _serverStateService = serverStateService ?? throw new ArgumentNullException(nameof(serverStateService));
        }

        protected IServerLogger Logger { get; set; }
        protected readonly IServerStateService<TOptions> _serverStateService;

        [JsonRpcIgnore]
        public async Task<TOptions> GetServerOptionsAsync() => await Task.FromResult(_serverStateService.Configuration);

        [JsonRpcIgnore]
        public async Task<IServerLogger> GetLoggerAsync() => await Task.FromResult(Logger);

        [JsonRpcIgnore]
        public async Task<IServerStateService<TOptions>> GetServerStateServiceAsync() => await Task.FromResult(_serverStateService);

        private IEnumerable<IJsonRpcSource> _clientProxies;

        [JsonRpcIgnore]
        public async Task<IEnumerable<IJsonRpcSource>> GetClientProxies() => await Task.FromResult(_clientProxies);

        [JsonRpcIgnore]
        public void Initialize(IEnumerable<IJsonRpcSource> clientProxies)
        {
            if (_clientProxies != null)
            {
                throw new InvalidOperationException("This instance was already intiialized. As a proxy service, its instancing should be scoped.");
            }

            _clientProxies = clientProxies;
            if (_clientProxies?.Any() ?? false)
            {
                RegisterClientProxyNotifications(clientProxies);
            }
        }

        protected abstract void RegisterClientProxyNotifications(IEnumerable<IJsonRpcSource> clientProxies);
    }
}
