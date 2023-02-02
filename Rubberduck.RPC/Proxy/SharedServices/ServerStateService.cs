using System;
using Rubberduck.RPC.Platform.Model;
using System.Diagnostics;
using System.Reflection;
using Rubberduck.RPC.Proxy.SharedServices.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Server.Configuration;

namespace Rubberduck.RPC.Proxy.SharedServices
{
    public class ServerStateService<TOptions> : IServerStateService<TOptions>
        where TOptions : SharedServerCapabilities, new()
    {
        private readonly Stopwatch _uptimeStopwatch;
        private readonly Process _hostProcess;
        private readonly string _assemblyName;
        private readonly string _assemblyVersion;

        private ServerState _currentState;
        public ServerStateService(TOptions configuration)
        {
            Configuration = configuration;

            var assemblyInfo = Assembly.GetEntryAssembly().GetName();
            _assemblyName = assemblyInfo.Name;
            _assemblyVersion = assemblyInfo.Version.ToString(3);
            _hostProcess = Process.GetCurrentProcess();

            _uptimeStopwatch = new Stopwatch();
        }

        public ServerStatus ServerStatus => _currentState?.Status ?? ServerStatus.Starting;

        public ServerState Info => _currentState is null 
            ? _currentState = new ServerState
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
                    Status = ServerStatus.Starting,
                    GC = GC.GetTotalMemory(false)
                }
            : new ServerState(_currentState);

        public TOptions Configuration { get; }
    }
}
