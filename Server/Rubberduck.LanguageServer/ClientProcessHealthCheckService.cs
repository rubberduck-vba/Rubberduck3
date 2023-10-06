using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.SettingsProvider;
using Rubberduck.SettingsProvider.Model;
using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading;

namespace Rubberduck.LanguageServer
{
    internal interface IHealthCheckService : IDisposable
    {
        void Start();
    }

    internal sealed class ClientProcessHealthCheckService : IHealthCheckService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly ISettingsProvider<LanguageServerSettings> _settingsProvider;
        private readonly Process _process;
        private readonly ILanguageServer _server;

        private readonly Timer _timer;

        public ClientProcessHealthCheckService(ILogger<ClientProcessHealthCheckService> logger, ISettingsProvider<LanguageServerSettings> settingsProvider, 
            Process process, ILanguageServer server)
        {
            _logger = logger;
            _settingsProvider = settingsProvider;
            _process = process;
            _server = server;

            _timer = new Timer(RunHealthCheck);
        }

        public bool IsAlive => !_process.HasExited;

        public void Dispose()
        {
            _timer.Dispose();
        }
        public void Start()
        {
            var settings = _settingsProvider.Settings;
            _logger.LogInformation(settings.TraceLevel.ToTraceLevel(), "Applying client process health check configuration.", $"Interval (ms): {settings.ClientHealthCheckInterval.TotalMilliseconds}");
            _timer.Change(settings.ClientHealthCheckInterval, settings.ClientHealthCheckInterval);
        }

// justification: crashing the process wouldn't even be a bad idea at that point.
#pragma warning disable VSTHRD100 // Avoid async void methods
        private async void RunHealthCheck(object? _)
#pragma warning restore VSTHRD100
        {
            var settings = _settingsProvider.Settings;
            if (_process.HasExited)
            {
                _logger.LogWarning(settings.TraceLevel.ToTraceLevel(), "Client process has exited.", $"ExitCode: {_process.ExitCode} ExitTime: {_process.ExitTime}");
                await _server.Exit.RunAsync(CancellationToken.None);
            }
            else
            {
                _logger.LogTrace(settings.TraceLevel.ToTraceLevel(), "Client process health check completed.", $"IsResponding: {_process.Responding} TotalProcessorTime: {_process.TotalProcessorTime}");
            }
        }
    }
}