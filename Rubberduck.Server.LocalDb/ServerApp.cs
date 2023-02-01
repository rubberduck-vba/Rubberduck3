using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Rubberduck.RPC;
using Rubberduck.RPC.Platform.Model;
using Rubberduck.RPC.Proxy.SharedServices;
using Rubberduck.Server.LocalDb.Services;

namespace Rubberduck.Server.LocalDb
{
    internal class ServerApp : BackgroundService
    {
        private readonly ServerCapabilities _configuration;

        private readonly Process _hostProcess;
        private readonly string _assemblyName;
        private readonly string _assemblyVersion;
        
        private readonly LocalDbServer _dbServer;
        private readonly LocalDbServerService _rpcServerProxy;

        private readonly Stopwatch _uptimeStopwatch;

        private ServerStatus _serverState = ServerStatus.Starting;

        internal ServerApp(ServerCapabilities configuration, LocalDbServer dbServer, LocalDbServerService rpcServerProxy)
        {
            _hostProcess = Process.GetCurrentProcess();

            var name = Assembly.GetExecutingAssembly().GetName();
            _assemblyName = name.Name;
            _assemblyVersion = name.Version.ToString(3);

            _dbServer = dbServer;
            _rpcServerProxy = rpcServerProxy;

            _configuration = configuration;

            _uptimeStopwatch = new Stopwatch();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Greetings();

            _rpcServerProxy.ClientConnected += ServerService_ClientConnected;
            _rpcServerProxy.ClientDisconnected += ServerService_ClientDisconnected;
            _rpcServerProxy.WillExit += ServerService_WillExit;

            _uptimeStopwatch.Start();
            return _dbServer.RunAsync(stoppingToken);
        }

        private void ServerService_ClientDisconnected(object sender, ClientInfo e)
        {
            Info.Disconnect(e.ProcessId, out _);
        }

        private void ServerService_ClientConnected(object sender, ClientInfo e)
        {
            Info.Connect(e);
        }

        private void ServerService_WillExit(object sender, EventArgs e)
        {
            _serverState = ServerStatus.Terminating;
        }

        private void Greetings()
        {
            Console.WriteLine(new string('*', 20));
            Console.WriteLine($"{_assemblyName} v{_assemblyVersion}");
            Console.WriteLine($"{_hostProcess.ProcessName} (ID {_hostProcess.Id}) | PriorityClass: {_hostProcess.PriorityClass} PriorityBoostEnabled: {_hostProcess.PriorityBoostEnabled}");
            Console.WriteLine(new string('*', 20));
        }

        /// <summary>
        /// Gets the current server info, including process-level details.
        /// </summary>
        internal ServerState Info => new ServerState
        {
            IsAlive = false,
            Name = _assemblyName,
            Version = _assemblyVersion,
            StartTime = _hostProcess.StartTime,
            UpTime = _uptimeStopwatch.Elapsed,
            Threads = _hostProcess.Threads.Count,
            ProcessId = _hostProcess.Id,
            WorkingSet = _hostProcess.WorkingSet64,
            PeakWorkingSet = _hostProcess.PeakWorkingSet64,
            Status = _serverState,
        };
    }
}
