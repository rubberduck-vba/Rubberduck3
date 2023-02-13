using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Proxy.SharedServices;
using Rubberduck.RPC.Proxy.SharedServices.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Server.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Rubberduck.RPC.Proxy.LocalDbServer
{
    /// <summary>
    /// A proxy service that implements <c>IServerProxy</c>.
    /// </summary>
    /// <remarks>
    /// Proxies should be stateless: the instance may be request-scoped.
    /// </remarks>
    public class LocalDbServerProxyService : ServerService<LocalDbServerCapabilities, InitializeParams<LocalDbServerCapabilities>>
    {
        public LocalDbServerProxyService(CancellationTokenSource serverTokenSource, IServerLogger logger, IServerStateService<LocalDbServerCapabilities> serverStateService)
            :base(logger, serverStateService)
        {
            var getConfig = new GetServerOptionsAsync<LocalDbServerCapabilities>(async () => await GetServerOptionsAsync());
            var getState = new GetServerStateInfoAsync(async () => (await GetServerStateServiceAsync()).Info);

            _initializedCommand = new InitializedCommand<LocalDbServerCapabilities>(logger, getConfig, getState);
            _exitCommand = new ExitCommand<LocalDbServerCapabilities>(serverTokenSource, logger, getConfig, getState);
        }

        private readonly InitializedCommand<LocalDbServerCapabilities> _initializedCommand;
        private async void Client_Initialized(object sender, InitializedParams e)
        {
            await _initializedCommand.ExecuteAsync(e);
        }

        private readonly ExitCommand<LocalDbServerCapabilities> _exitCommand;
        private async void Client_RequestExit(object sender, EventArgs e)
        {
            await _exitCommand.ExecuteAsync();
        }

        protected override void RegisterClientProxyNotifications(IEnumerable<IJsonRpcSource> clientProxies)
        {
            var client = clientProxies.OfType<IServerProxyClient<LocalDbServerCapabilities>>().Single();
            client.Initialized += Client_Initialized;
            client.RequestExit += Client_RequestExit;
        }
    }
}
