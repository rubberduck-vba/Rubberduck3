using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Proxy.LocalDbServer;
using Rubberduck.RPC.Proxy.SharedServices.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Console.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Console.Commands;
using Rubberduck.RPC.Proxy.SharedServices.Console.Configuration;
using Rubberduck.RPC.Proxy.SharedServices.Server.Commands;
using System.Threading;

namespace Rubberduck.Server.LocalDb.Services
{
    internal class ServerServiceFactory : IServerServiceFactory<LocalDbServerService, ServerCapabilities, ILocalDbServerProxyClient>
    {
        private readonly CancellationToken _serverToken;
        private readonly IServerLogger _logger;
        private readonly ILocalDbServerProxyClient _clientProxy;
        private readonly GetServerOptions<ServerCapabilities> _getConfiguration;
        private readonly GetServerStateInfo _getServerState;
        private readonly IServerConsoleService<ServerConsoleOptions> _consoleService;

        public ServerServiceFactory(CancellationToken serverToken, IServerLogger logger, ILocalDbServerProxyClient clientProxy, GetServerOptions<ServerCapabilities> getConfiguration, GetServerStateInfo getServerState) 
        {
            _serverToken = serverToken;
            _logger = logger;
            _clientProxy = clientProxy;
            _getConfiguration = getConfiguration;
            _getServerState = getServerState;
        }

        public LocalDbServerService Create(ServerCommands<ServerCapabilities> commands)
            => new LocalDbServerService(_serverToken, _logger, _clientProxy, _getConfiguration, _getServerState, _consoleService);

        public ServerConsoleService<ServerConsoleOptions> Create(ServerConsoleCommands commands)
            => new ServerConsoleService<ServerConsoleOptions>(_getConfiguration().ConsoleOptions, _getServerState);
    }
}
