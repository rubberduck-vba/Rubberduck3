﻿using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Common;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model;
using System;
using System.Diagnostics;
using System.Threading;

namespace Rubberduck.ServerPlatform
{
    public interface IHealthCheckService<TSettings> : IDisposable
        where TSettings : IHealthCheckSettingsProvider
    {
        event EventHandler<EventArgs>? ChildProcessExited;
        void Start();
    }

    public sealed class ClientProcessHealthCheckService<TSettings> : IHealthCheckService<TSettings>, IDisposable
        where TSettings : IHealthCheckSettingsProvider
    {
        private readonly ILogger _logger;
        private readonly ISettingsProvider<TSettings> _settingsProvider;
        private readonly Process _process;

        private readonly Timer _timer;

        public event EventHandler<EventArgs>? ChildProcessExited = delegate { };

        private TraceLevel TraceLevel => _settingsProvider.Settings.ServerTraceLevel.ToTraceLevel();
        private TimeSpan Interval => _settingsProvider.Settings.ClientHealthCheckInterval;

        public ClientProcessHealthCheckService(
            ILogger<ClientProcessHealthCheckService<TSettings>> logger, 
            ISettingsProvider<TSettings> settingsProvider, 
            Process process)
        {
            _logger = logger;
            _settingsProvider = settingsProvider;
            _process = process;

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

        private bool _didNotifyExit = false;
        private void RunHealthCheck(object? _)
        {
            if (TimedAction.TryRun(() =>
            {
                if (!_didNotifyExit && _process.HasExited)
                {
                    _logger.LogWarning(TraceLevel, "Client process has exited.");
                    ChildProcessExited?.Invoke(this, EventArgs.Empty);
                    _didNotifyExit = true;
                }
            }, out var elapsed, out var exception))
            {
                var message = $"Client process health check completed. IsResponding: {_process.Responding} TotalProcessorTime: {_process.TotalProcessorTime}";
                _logger.LogPerformance(TraceLevel, message, elapsed);
                _didNotifyExit = false;
            }
            else if (exception != null)
            {
                _logger.LogError(TraceLevel, exception);
            }
        }
    }
}