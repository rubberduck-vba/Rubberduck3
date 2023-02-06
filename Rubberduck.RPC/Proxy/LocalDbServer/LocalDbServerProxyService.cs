using Rubberduck.RPC.Platform.Model;
using Rubberduck.RPC.Proxy.SharedServices;
using Rubberduck.RPC.Proxy.SharedServices.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Server.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Server.Commands;
using Rubberduck.RPC.Proxy.SharedServices.Server.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.RPC.Proxy.LocalDbServer
{
    /// <summary>
    /// A proxy service that implements <c>IServerProxy</c>.
    /// </summary>
    /// <remarks>
    /// Proxies should be stateless: the instance may be request-scoped.
    /// </remarks>
    public class LocalDbServerProxyService : IServerProxy<LocalDbServerCapabilities>
    {
        public LocalDbServerProxyService(CancellationTokenSource serverTokenSource, IServerLogger logger, IServerStateService<LocalDbServerCapabilities> serverStateService,
            IServerProxyClient<LocalDbServerCapabilities> client)
        {
            ServerStateService = serverStateService;
            Logger = logger;

            client.RequestExit += Client_RequestExit;
            client.Initialized += Client_Initialized;

            var getConfig = new GetServerOptions<LocalDbServerCapabilities>(() => ServerOptions);
            var getState = new GetServerStateInfo(() => ServerStateService.Info);

            _initializedCommand = new InitializedCommand<LocalDbServerCapabilities>(logger, getConfig, getState);
            _exitCommand = new ExitCommand(serverTokenSource, logger, getConfig, getState);
        }

        private readonly InitializedCommand<LocalDbServerCapabilities> _initializedCommand;
        private async void Client_Initialized(object sender, InitializedParams e)
        {
            await _initializedCommand.ExecuteAsync(e);
        }

        private readonly ExitCommand _exitCommand;
        private async void Client_RequestExit(object sender, EventArgs e)
        {
            await _exitCommand.ExecuteAsync();
        }

        public IServerStateService<LocalDbServerCapabilities> ServerStateService { get; }

        public IServerLogger Logger { get; }

        public LocalDbServerCapabilities ServerOptions { get; }

        public event EventHandler WillExit;
        public Task OnWillExitAsync() => Task.Run(() => WillExit?.Invoke(this, EventArgs.Empty));

        private readonly InitializeCommand<LocalDbServerCapabilities, LocalDbServerCapabilities> _initializeCommand; // TODO implement LocalDbClientCapabilities?
        public async Task<InitializeResult<LocalDbServerCapabilities>> InitializeAsync(InitializeParams<LocalDbServerCapabilities> parameter, CancellationToken token)
        {
            return await _initializeCommand.ExecuteAsync(parameter, token);
        }

        public async Task<ServerState> RequestServerInfoAsync()
        {
            return await Task.FromResult(ServerStateService.Info);
        }
    }
}
