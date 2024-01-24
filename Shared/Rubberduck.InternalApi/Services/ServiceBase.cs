﻿using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Common;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Settings;
using Rubberduck.InternalApi.Settings.Model;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Rubberduck.InternalApi.Services
{
    public abstract class ServiceBase
    {
        protected readonly ILogger _logger;
        protected readonly PerformanceRecordAggregator _performance;

        protected RubberduckSettingsProvider SettingsProvider { get; init; }

        protected ServiceBase(ILogger logger, RubberduckSettingsProvider settingsProvider, PerformanceRecordAggregator performance)
        {
            _logger = logger;
            _performance = performance;
            SettingsProvider = settingsProvider;
        }

        public RubberduckSettings Settings => SettingsProvider.Settings;
        public TraceLevel TraceLevel => SettingsProvider.Settings.LoggerSettings.TraceLevel.ToTraceLevel();

        public void LogPerformance(TimeSpan elapsed, string? message = default, [CallerMemberName] string? name = default, PerformanceLoggerMode mode = PerformanceLoggerMode.LogAndAggregate)
        {
            var verbosity = TraceLevel;
            if (verbosity == TraceLevel.Off)
            {
                return;
            }

            name ??= "(name:null)";
            message = message is null ? $"{name} completed." : $"{name} completed; message: {message}";

            if (_performance != null && Settings.LoggerSettings.AggregatePerformanceLogs)
            {
                _performance.Add(new() { Name = name, Elapsed = elapsed });
            }

            if (mode != PerformanceLoggerMode.AggregateOnly)
            {
                _logger.LogPerformance(verbosity, message, elapsed);
            }
        }

        public void LogException(Exception exception, string? message = default)
        {
            var verbosity = TraceLevel;
            if (verbosity == TraceLevel.Off)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                _logger.LogError(verbosity, exception);
            }
            else
            {
                _logger.LogError(verbosity, exception, message);
            }
        }

        public void LogWarning(string message, string? verbose = default)
        {
            var verbosity = TraceLevel;
            if (verbosity == TraceLevel.Off)
            {
                return;
            }

            _logger.LogWarning(verbosity, message, verbose);
        }

        public void LogInformation(string message, string? verbose = default)
        {
            var verbosity = TraceLevel;
            if (verbosity == TraceLevel.Off)
            {
                return;
            }

            _logger.LogInformation(verbosity, message, verbose);
        }

        public void LogDebug(string message, string? verbose = default)
        {
            var verbosity = TraceLevel;
            if (verbosity == TraceLevel.Off)
            {
                return;
            }

            _logger.LogDebug(verbosity, message, verbose);
        }

        public void LogTrace(string message, string? verbose = default)
        {
            var verbosity = TraceLevel;
            if (verbosity == TraceLevel.Off)
            {
                return;
            }

            _logger.LogTrace(verbosity, message, verbose);
        }

        /// <summary>
        /// Logs the execution performance of an action delegate, handling any exceptions thrown.
        /// </summary>
        /// <param name="action">The action delegate to clock.</param>
        /// <param name="exception">Any exception thrown while executing the provided action delegate.</param>
        /// <param name="name">A name for the action, defaults to the calling method's name.</param>
        /// <returns><c>true</c> if the action completes successfully without throwing.</returns>
        /// <remarks>the returned exception, if not null, has already been logged.</remarks>
        public bool TryRunAction(Action action, out Exception? exception, [CallerMemberName] string? name = default)
        {
            var verbosity = TraceLevel;
            if (TimedAction.TryRun(action, out var elapsed, out exception))
            {
                LogPerformance(elapsed, name: name);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Logs the execution performance of an action delegate, handling any exceptions thrown.
        /// </summary>
        /// <param name="action">The action delegate to clock.</param>
        /// <param name="name">A name for the action, defaults to the calling method's name.</param>
        /// <returns><c>true</c> if the action completes successfully without throwing.</returns>
        /// <remarks>the returned exception, if not null, has already been logged.</remarks>
        public bool TryRunAction(Action action, [CallerMemberName] string? name = default)
        {
            var verbosity = TraceLevel;
            if (TimedAction.TryRun(action, out var elapsed, out var exception))
            {
                LogPerformance(elapsed, name: name);
                return true;
            }
            else if (exception is not null)
            {
                OnError(exception, $"TryRunAction failed: [{name}]");
            }

            return false;
        }

        protected virtual void OnError(Exception exception, string? message)
        {
            LogException(exception, message);
        }
    }
}