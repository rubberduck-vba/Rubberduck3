using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Proxy.SharedServices.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Console.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Console.Configuration;
using Rubberduck.RPC.Proxy.SharedServices.Server.Commands;
using StreamJsonRpc;
using System.Threading;

namespace Rubberduck.Server.LocalDb.Services
{
    internal class ServerServiceFactory : IServerServiceFactory<LocalDbServerService, ServerCapabilities>
    {
        private readonly CancellationToken _serverToken;
        private readonly IServerLogger _logger;
        private readonly GetServerOptions<ServerCapabilities> _getConfiguration;
        private readonly GetServerStateInfo _getServerState;
        private readonly IServerConsoleService<ServerConsoleOptions> _consoleService;

        public ServerServiceFactory(CancellationToken serverToken, IServerLogger logger, IJsonRpcClientProxy clientProxy, GetServerOptions<ServerCapabilities> getConfiguration, GetServerStateInfo getServerState) 
        {
            _serverToken = serverToken;
            _logger = logger;
            _getConfiguration = getConfiguration;
            _getServerState = getServerState;

            _consoleService = Create();
        }

        public LocalDbServerService Create(ServerCommands<ServerCapabilities> commands)
            => new LocalDbServerService(_serverToken, _logger, null, _getConfiguration, _getServerState, _consoleService);

        public ServerConsoleService<ServerConsoleOptions> Create()
            => new ServerConsoleService<ServerConsoleOptions>(_getConfiguration().ConsoleOptions, _getServerState);
    }
}
