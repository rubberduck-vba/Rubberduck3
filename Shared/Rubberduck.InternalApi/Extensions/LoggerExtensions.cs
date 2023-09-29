using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Rubberduck.InternalApi.Extensions
{
    public static class LoggerExtensions
    {
        public class LogPerformanceOptions
        {
            private static readonly SortedDictionary<TimeSpan, LogLevel> _defaultMappings = new()
            {
                [TimeSpan.Zero] = LogLevel.Trace,
                [TimeSpan.FromMilliseconds(100)] = LogLevel.Information,
                [TimeSpan.FromSeconds(1)] = LogLevel.Warning,
                [TimeSpan.FromMinutes(1)] = LogLevel.Error,
                [TimeSpan.FromMinutes(5)] = LogLevel.Critical,
            };
            public static LogPerformanceOptions Default { get; } = new() { Mappings = _defaultMappings };

            public IDictionary<TimeSpan, LogLevel> Mappings { get; set; } = new SortedDictionary<TimeSpan, LogLevel>();

            public LogLevel GetLogLevel(TimeSpan elapsed)
            {
                var level = LogLevel.Trace;
                foreach (var key in Mappings.Keys.OrderBy(e => e))
                {
                    if (elapsed < key)
                    {
                        level = Mappings[key];
                    }
                    else
                    {
                        break;
                    }
                }
                return level;
            }
        }

        /// <summary>
        /// Logs the specified completion <c>message</c> at a level that depends on the default elapsed <c>TimeSpan</c> mapping options,
        /// along with the <c>elapsed</c> time span itself when <c>level</c> is <c>TraceLevel.Verbose</c>.
        /// </summary>
        /// <remarks>Use <c>TimedAction</c> to get the elapsed <c>TimeSpan</c> of a given set of instructions.</remarks>
        public static void LogPerformance(this ILogger logger, string message, TimeSpan elapsed, TraceLevel level)
            => logger.LogPerformance(message, elapsed, LogPerformanceOptions.Default, level);

        /// <summary>
        /// Logs the specified completion <c>message</c> at a level that depends on elapsed <c>TimeSpan</c> mappings specified in <c>options</c>,
        /// along with the <c>elapsed</c> time span itself when <c>level</c> is <c>TraceLevel.Verbose</c>.
        /// </summary>
        /// <remarks>Use <c>TimedAction</c> to get the elapsed <c>TimeSpan</c> of a given set of instructions.</remarks>
        public static void LogPerformance(this ILogger logger, string message, TimeSpan elapsed, LogPerformanceOptions options, TraceLevel level)
        {
            var logLevel = options.GetLogLevel(elapsed);
            logger.Log(logLevel, message, $"Elapsed: {elapsed}", level);
        }

        /// <summary>
        /// Logs the specified <c>message</c> at <c>Trace</c> level, along with the <c>verbose</c> message when <c>level</c> is <c>TraceLevel.Verbose</c>.
        /// </summary>
        public static void LogTrace(this ILogger logger, string message, string verbose, TraceLevel level) => 
            logger.Log(LogLevel.Trace, message, verbose, level);

        /// <summary>
        /// Logs the specified <c>message</c> at <c>Debug</c> level, along with the <c>verbose</c> message when <c>level</c> is <c>TraceLevel.Verbose</c>.
        /// </summary>
        public static void LogDebug(this ILogger logger, string message, string verbose, TraceLevel level) =>
            logger.Log(LogLevel.Debug, message, verbose, level);

        /// <summary>
        /// Logs the specified <c>message</c> at <c>Information</c> level, along with the <c>verbose</c> message when <c>level</c> is <c>TraceLevel.Verbose</c>.
        /// </summary>
        public static void LogInformation(this ILogger logger, string message, string verbose, TraceLevel level) =>
            logger.Log(LogLevel.Information, message, verbose, level);

        /// <summary>
        /// Logs the specified <c>message</c> at <c>Warning</c> level, along with the <c>verbose</c> message when <c>level</c> is <c>TraceLevel.Verbose</c>.
        /// </summary>
        public static void LogWarning(this ILogger logger, string message, string verbose, TraceLevel level) =>
            logger.Log(LogLevel.Warning, message, verbose, level);

        /// <summary>
        /// Logs the <c>Message</c> of the specified <c>Exception</c> at <c>Warning</c> level, along with the stack trace when <c>level</c> is <c>TraceLevel.Verbose</c>.
        /// </summary>
        public static void LogWarning(this ILogger logger, Exception exception, TraceLevel level) =>
            logger.Log(LogLevel.Warning, exception.Message, exception.StackTrace, level);

        /// <summary>
        /// Logs the <c>Message</c> of the specified <c>Exception</c> at <c>Error</c> level, along with the stack trace when <c>level</c> is <c>TraceLevel.Verbose</c>.
        /// </summary>
        public static void LogError(this ILogger logger, Exception exception, TraceLevel level) =>
            logger.Log(LogLevel.Error, exception.Message, exception.StackTrace, level);

        /// <summary>
        /// Logs the <c>Message</c> of the specified <c>Exception</c> at <c>Critical</c> level, along with the stack trace when <c>level</c> is <c>TraceLevel.Verbose</c>.
        /// </summary>
        public static void LogCritical(this ILogger logger, Exception exception, TraceLevel level) =>
            logger.Log(LogLevel.Critical, exception, exception.Message, exception.StackTrace, level);

        private static void Log(this ILogger logger, LogLevel logLevel, string message, string verbose, TraceLevel level)
        {
            if (level == TraceLevel.Off)
            {
                return;
            }

            var logMessage = message;
            if (level == TraceLevel.Verbose)
            {
                logMessage = $"{message} | {verbose}";
            }

            logger.Log(logLevel, logMessage);
        }
    }
}
