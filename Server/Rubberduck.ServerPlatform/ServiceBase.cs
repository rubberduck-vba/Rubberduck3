using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Common;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Rubberduck.ServerPlatform
{
    public class ServerPlatformServiceHelper : ServiceBase
    {
        public ServerPlatformServiceHelper(ILogger logger, ISettingsProvider<RubberduckSettings> settingsProvider, IWorkDoneProgressStateService workdone) 
            : base(logger, settingsProvider, workdone) { }
    }

    public abstract class ServiceBase
    {
        private readonly ILogger _logger;
        private readonly ISettingsProvider<RubberduckSettings> _settingsProvider;
        private readonly IWorkDoneProgressStateService? _workdone;

        protected ServiceBase(ILogger logger, ISettingsProvider<RubberduckSettings> settingsProvider, IWorkDoneProgressStateService? workdone)
        {
            _logger = logger;
            _settingsProvider = settingsProvider;
            _workdone = workdone;
        }

        public RubberduckSettings Settings => _settingsProvider.Settings;
        public TraceLevel TraceLevel => _settingsProvider.Settings.GeneralSettings.TraceLevel.ToTraceLevel();

        public void LogPerformance(TimeSpan elapsed, string? message = default, [CallerMemberName]string? name = default)
        {
            var verbosity = TraceLevel;
            if (verbosity == TraceLevel.Off)
            {
                return;
            }

            message ??= $"{name} completed.";
            _logger.LogPerformance(verbosity, message, elapsed);
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
        public bool TryRunAction(Action action, out Exception? exception, [CallerMemberName]string? name = default)
        {
            var verbosity = TraceLevel;
            if (TimedAction.TryRun(action, out var elapsed, out exception))
            {
                LogPerformance(elapsed, name:name);
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
                LogException(exception, $"TryRunAction failed: [{name}]");
            }

            return false;
        }


        /// <summary>
        /// Logs the execution performance of an action delegate, without handling any exceptions thrown.
        /// </summary>
        /// <param name="action">The action delegate to clock.</param>
        /// <param name="name">A name for the action, defaults to the calling method's name.</param>
        public void RunAction(Action action, [CallerMemberName] string? name = default)
        {
            var elapsed = TimedAction.Run(action);
            LogPerformance(elapsed, name:name);
        }

        public virtual void OnProgress(ProgressToken token, WorkDoneProgressReport value) => _workdone?.OnProgress(token, value);
    }
}