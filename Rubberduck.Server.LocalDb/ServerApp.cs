using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Rubberduck.RPC;
using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Proxy.SharedServices;
using Rubberduck.RPC.Proxy.SharedServices.Abstract;
using Rubberduck.Server.LocalDb.Services;

namespace Rubberduck.Server.LocalDb
{
    internal class ServerApp : BackgroundService
    {
        private readonly IServerStateService<ServerCapabilities> _serverStateService;
        
        private readonly IJsonRpcServer _dbServer;
        private readonly LocalDbServerService _rpcServerProxy;

        public ServerApp(IJsonRpcServer server, IServerStateService<ServerCapabilities> stateService, IEnumerable<IJsonRpcTarget> proxies)
        {
            _dbServer = server;
            _serverStateService = stateService;
            _rpcServerProxy = proxies.OfType<LocalDbServerService>().SingleOrDefault();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Greetings();

            _rpcServerProxy.ClientConnected += ServerService_ClientConnected;
            _rpcServerProxy.ClientDisconnected += ServerService_ClientDisconnected;
            _rpcServerProxy.WillExit += ServerService_WillExit;

            return _dbServer.StartAsync(stoppingToken);
        }

        private void ServerService_ClientDisconnected(object sender, ClientInfo e)
        {
            _serverStateService.Info.Disconnect(e.ProcessId, out _);
        }

        private void ServerService_ClientConnected(object sender, ClientInfo e)
        {
            _serverStateService.Info.Connect(e);
        }

        private void ServerService_WillExit(object sender, EventArgs e)
        {
            _serverStateService.Info.Status = ServerStatus.Terminating;
        }

        private void Greetings()
        {
            var state = _serverStateService.Info;

            Console.WriteLine(new string('*', 20));
            Console.WriteLine($"{state.Name} v{state.Version}");
            Console.WriteLine($"Process ID: {state.ProcessId}");
            Console.WriteLine(new string('*', 20));
        }
    }
}
