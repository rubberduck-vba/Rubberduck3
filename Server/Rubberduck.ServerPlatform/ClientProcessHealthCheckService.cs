﻿using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Common;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.InternalApi.Settings.Model;
using System;
using System.Diagnostics;
using System.Threading;

namespace Rubberduck.ServerPlatform
{
    public interface IHealthCheckService<TSettings> : IDisposable
        where TSettings : RubberduckSetting, IHealthCheckSettingsProvider
    {
        event EventHandler<EventArgs>? ChildProcessExited;
        void Start();
    }

    public sealed class ClientProcessHealthCheckService<TSettings> : ServerPlatformServiceBase, IHealthCheckService<TSettings>, IDisposable
        where TSettings : RubberduckSetting, IHealthCheckSettingsProvider
    {
        private readonly ILogger _logger;
        private readonly Process _process;

        private readonly Timer _timer;
        private readonly Func<TSettings> _settingsProvider;

        public event EventHandler<EventArgs>? ChildProcessExited = delegate { };
        private TimeSpan Interval => HealthCheckSettings.ClientHealthCheckInterval;

        public ClientProcessHealthCheckService(
            ILogger<ClientProcessHealthCheckService<TSettings>> logger, 
            RubberduckSettingsProvider settings,
            Func<TSettings> settingsProvider,
            IWorkDoneProgressStateService workdone,
            Process process,
            PerformanceRecordAggregator performance)
            : base(logger, settings, workdone, performance)
        {
            _logger = logger;
            _process = process;
            _settingsProvider = settingsProvider;

            _timer = new Timer(RunHealthCheck);
        }

        public bool IsAlive => !_process.HasExited;

        public TSettings HealthCheckSettings => _settingsProvider.Invoke();

        public void Dispose()
        {
            _timer.Dispose();
        }

        public void Start()
        {
            var interval = Interval;
            var timer = _timer;

            TryRunAction(() =>
            {
                LogInformation("Applying client process health check configuration.", $"Interval (ms): {interval.TotalMilliseconds}");
                timer.Change(TimeSpan.Zero, interval);
            }, "ClientProcessHealthCheckService.Start");
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