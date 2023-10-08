using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.General;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using Rubberduck.InternalApi.Common;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.SettingsProvider;
using Rubberduck.SettingsProvider.Model;
using System;
using System.Diagnostics;
using System.Threading;

namespace Rubberduck.LanguageServer
{
    public interface IHealthCheckService : IDisposable
    {
        void Start();
    }

    internal sealed class ClientProcessHealthCheckService : IHealthCheckService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly ISettingsProvider<LanguageServerSettings> _settingsProvider;
        private readonly Process _process;

        private readonly Timer _timer;
        private readonly IExitHandler _exitHandler;

        private TraceLevel TraceLevel => _settingsProvider.Settings.TraceLevel.ToTraceLevel();
        private TimeSpan Interval => _settingsProvider.Settings.ClientHealthCheckInterval;

        public ClientProcessHealthCheckService(ILogger<ClientProcessHealthCheckService> logger, ISettingsProvider<LanguageServerSettings> settingsProvider, 
            Process process, IExitHandler exitHandler)
        {
            _logger = logger;
            _settingsProvider = settingsProvider;
            _process = process;
            _exitHandler = exitHandler;

            _timer = new Timer(RunHealthCheck);
        }

        public bool IsAlive => !_process.HasExited;

        public void Dispose()
        {
            _timer.Dispose();
        }

        public void Start()
        {
            var traceLevel = TraceLevel;
            var interval = Interval;

            var logger = _logger;
            var timer = _timer;

            if (TimedAction.TryRun(() =>
            {
                logger.LogInformation(traceLevel, "Applying client process health check configuration.", $"Interval (ms): {interval.TotalMilliseconds}");
                timer.Change(TimeSpan.Zero, interval);
            }, out var elapsed, out var exception))
            {
                logger.LogPerformance(traceLevel, "Applied client process health check configuration.", elapsed);
            }
            else if (exception != null)
            {
                logger.LogError(traceLevel, exception, "Could not apply client process health check configuration.");
                throw exception;
            }
        }

        private void RunHealthCheck(object? _)
        {
            if (TimedAction.TryRun(() =>
            {
                if (_process.HasExited)
                {
                    _logger.LogWarning(TraceLevel, "Client process has exited.");
                    _ = _exitHandler.Handle(new ExitParams(), CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();
                }
            }, out var elapsed, out var exception))
            {
                var message = $"Client process health check completed. IsResponding: {_process.Responding} TotalProcessorTime: {_process.TotalProcessorTime}";
                _logger.LogPerformance(TraceLevel, message, elapsed);
            }
            else if (exception != null)
            {
                _logger.LogError(TraceLevel, exception);
            }
        }
    }
}