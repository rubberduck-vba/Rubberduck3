using Rubberduck.RPC;
using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Model;
using Rubberduck.RPC.Proxy.LocalDbServer;
using Rubberduck.RPC.Proxy.SharedServices;
using Rubberduck.RPC.Proxy.SharedServices.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Console.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Console.Configuration;
using Rubberduck.RPC.Proxy.SharedServices.Server.Commands;
using Rubberduck.RPC.Proxy.SharedServices.Server.Configuration;
using Rubberduck.Server.LocalDb.Properties;
using Rubberduck.Server.LocalDb.Services;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.Server.LocalDb
{
    internal class App
    {
        private readonly ServerCapabilities _configuration;

        private readonly Process _hostProcess;
        private readonly AssemblyName _assemblyInfo;
        
        private readonly LocalDbServer _dbServer;
        private readonly LocalDbServerService _rpcServer;

        private readonly Stopwatch _uptimeStopwatch;

        private readonly CancellationTokenSource _serverTokenSource;
        private readonly NamedPipeStreamFactory _rpcStreamFactory;
        private readonly IServerServiceFactory<LocalDbServerService, ServerCapabilities> _serverServiceFactory;

        private readonly IServerLogger _logger;
        private readonly ILocalDbServerProxyClient _clientProxy;

        private readonly ServerConsoleService<ServerConsoleOptions> _consoleProxyService;

        internal App(ServerCapabilities configuration)
        {
            _configuration = configuration;

            GetServerStateInfo getInfo = () => Info;
            GetServerOptions<ServerCapabilities> getConfig = () => _configuration;

            _hostProcess = Process.GetCurrentProcess();
            _assemblyInfo = Assembly.GetExecutingAssembly().GetName();
            _uptimeStopwatch = new Stopwatch();

            _consoleProxyService = new ServerConsoleService<ServerConsoleOptions>(_configuration.ConsoleOptions, getInfo);
            _logger = new ServerLogger(exception => _consoleProxyService.Logger.OnError(exception), (level, message, verbose) => _consoleProxyService.Log(level, message, verbose));


            _serverTokenSource = new CancellationTokenSource();
            _rpcStreamFactory = new NamedPipeStreamFactory(Settings.Default.JsonRpcPipeName, Settings.Default.MaxConcurrentRequests);

            _serverServiceFactory = new ServerServiceFactory(_serverTokenSource.Token, _logger, null, getConfig, getInfo);

            _dbServer = new LocalDbServer(Info, getInfo, _rpcStreamFactory, _serverServiceFactory, _serverTokenSource);
            _rpcServer = new LocalDbServerService(_serverTokenSource.Token, _logger, _clientProxy, getConfig, getInfo, _consoleProxyService);
        }

        /// <summary>
        /// Starts the LocalDb RPC server.
        /// </summary>
        public async Task StartAsync()
        {
            var serverCommands = new ServerCommands<SharedServerCapabilities>();

            _rpcServer.ClientConnected += ServerService_ClientConnected;
            _rpcServer.ClientDisconnected += ServerService_ClientDisconnected;
            _rpcServer.WillExit += ServerService_WillExit;

            _uptimeStopwatch.Start();
            await _dbServer.StartAsync(_serverTokenSource.Token);
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
            // TODO notify any remaining clients that they're losing their server connection..
            
        }

        private ILocalDbServerProxyClient GetClientProxy(NamedPipeStreamFactory rpcStreamFactory)
        {
            
            return null;
        }

        /// <summary>
        /// Gets the current server info, including process-level details.
        /// </summary>
        internal ServerState Info => new ServerState
        {
            IsAlive = false,
            Name = _assemblyInfo.Name,
            Version = _assemblyInfo.Version.ToString(3),
            StartTime = _hostProcess.StartTime,
            UpTime = _uptimeStopwatch.Elapsed,
            Threads = _hostProcess.Threads.Count,
            ProcessId = _hostProcess.Id,
            WorkingSet = _hostProcess.WorkingSet64,
            PeakWorkingSet = _hostProcess.PeakWorkingSet64,
            Status = ServerStatus.Started,
        };
    }
}
