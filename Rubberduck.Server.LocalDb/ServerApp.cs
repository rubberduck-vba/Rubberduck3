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

            _rpcServerProxy.WillExit += ServerService_WillExit;

            return _dbServer.StartAsync(stoppingToken);
        }

        private void ServerService_WillExit(object sender, EventArgs e)
        {
            _serverStateService.Info.Status = ServerStatus.Terminating;
        }

        private void Greetings()
        {
            var state = _serverStateService.Info;

            Console.WriteLine(new string('*', 80));
            Console.WriteLine(@"

                (#########
              ##..........##(
             ##.......##/...#######
             ##...........##(///##
  (###        ##..........,####                                     @@@@@@@
  #*...####    ,##.........#          @@@@@@@@@@@  .@@@@@@@@@@.    @@/**,,@@*.
 ##..........,*,............##*        @@@,    @@/.  @@*.    @@/        *@@@/,
 ##...........................##       @@@@@@@@@/*   @@*.    @@@,      &@@@@,
 ##............................##      @@@*,,@@@(    @@*.    @@(,         ,@@*
  #*...........................##    (@@@@@@   @@@@@@@@@@@@@@@/*  @@@@@@@@@@/*
   ##.........................##      .,****,    ,*,,,*****,,.      ,******,
    *##.....................##.
       (###(..........,####

");
            Console.WriteLine(new string('*', 80));
            Console.WriteLine($"{state.Name} v{state.Version} (C)2014-{DateTime.Today.Year} Rubberduck project contributors");
            Console.WriteLine($"Process ID: {state.ProcessId}");
        }
    }
}
