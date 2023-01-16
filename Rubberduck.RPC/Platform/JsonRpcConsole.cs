using NLog;
using Rubberduck.InternalApi.RPC;
using System;
using System.Threading;

namespace Rubberduck.RPC.Platform
{
    public class ConsoleMessageEventArgs : EventArgs
    {
        public ConsoleMessageEventArgs(int id, DateTime timestamp, LogLevel level, string message, string verbose, Exception exception = null)
        {
            Id = id;
            Timestamp = timestamp;

            Level = level;
            Message = message;
            Verbose = verbose;

            Exception = exception;
            IsError = exception != null;
        }

        public int Id { get; }

        public DateTime Timestamp { get; }

        /// <summary>
        /// <c>true</c> if the <c>Exception</c> property contains error information.
        /// </summary>
        public bool IsError { get; }

        /// <summary>
        /// The message level. Client may use it to filter messages displayed.
        /// </summary>
        public LogLevel Level { get; }

        /// <summary>
        /// The message text.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Additional content shown when verbose tracing is enabled.
        /// </summary>
        public string Verbose { get; }
        /// <summary>
        /// An exception with additional error information. <c>null</c> if there was no error.
        /// </summary>
        public Exception Exception { get; }
    }

    public interface IJsonRpcConsoleEvents
    {
        /// <summary>
        /// An event that signals all messages to its registered handlers.
        /// </summary>
        event EventHandler<ConsoleMessageEventArgs> Message;
    }


    public interface IJsonRpcConsole : IJsonRpcConsoleEvents
    {
        /// <summary>
        /// Gets/sets the verbosity level of this server.
        /// </summary>
        string Trace { get; set; }

        /// <summary>
        /// Gets/sets whether the server outputs to this console.
        /// </summary>
        /// <remarks>
        /// Internal logging remains enabled.
        /// </remarks>
        bool IsEnabled { get; set; }
        bool IsVerbose { get; }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="verbose"></param>
        void Log(LogLevel level, string message, string verbose = null);
        /// <summary>
        /// Logs an error.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="verbose"></param>
        void Log(Exception exception, LogLevel level, string message, string verbose = null);
    }

    public interface IJsonRpcConsole<TOptions> : IJsonRpcConsole
        where TOptions : class, new()
    {
        /// <summary>
        /// Configuration options for this JSON-RPC console service.
        /// </summary>
        TOptions Configuration { get; }
    }

    public class JsonRpcConsole : IJsonRpcConsole
    {
        protected ILogger InternalLogger { get; } = LogManager.GetLogger($"{nameof(IJsonRpcConsole)}.Internal");

        public event EventHandler<ConsoleMessageEventArgs> Message;

        private int _nextMessageId = 0;

        public string Trace { get; set; }

        public bool IsEnabled { get; set; }
        public bool IsVerbose => Trace == Constants.TraceValue.Verbose;

        private bool CanLog => IsEnabled && Trace != Constants.TraceValue.Off;

        public void Log(LogLevel level, string message, string verbose = null)
        {
            LogInternal(level, message, verbose);

            if (!CanLog)
            {
                return;
            }

            var id = Interlocked.Increment(ref _nextMessageId);
            OnMessage(id, level, message, verbose);
        }

        public void Log(Exception exception, LogLevel level, string message, string verbose = null)
        {
            LogInternal(level, message, verbose, exception);

            if (!CanLog)
            {
                return;
            }

            var id = Interlocked.Increment(ref _nextMessageId);
            OnMessage(id, level, message, verbose, exception);
        }

        protected virtual string StdFormat(int id, LogLevel level, string content)
        {
            return $"{id:000000000} {DateTime.Now:O} {level.ToString().ToUpperInvariant()} {content}";
        }

        protected virtual void StdOut(int id, LogLevel level, string message, string verbose = null)
        {
            Console.Out.WriteLine(StdFormat(id, level, message));
            if (IsVerbose && !string.IsNullOrWhiteSpace(verbose))
            {
                Console.Out.WriteLine(StdFormat(id, level, verbose));
            }
        }

        protected virtual void StdErr(int id, LogLevel level, string message, string verbose = null)
        {
            Console.Error.WriteLine(StdFormat(id, level, message));
            if (IsVerbose && !string.IsNullOrWhiteSpace(verbose))
            {
                Console.Error.WriteLine(StdFormat(id, level, verbose));
            }
        }

        protected virtual void LogInternal(LogLevel level, string message, string verbose)
            => InternalLogger.Log(level, message + (IsVerbose ? " " + verbose : null));

        protected virtual void LogInternal(LogLevel level, string message, string verbose, Exception exception)
            => InternalLogger.Log(level, exception, message + (IsVerbose ? " " + verbose : null));

        private void OnMessage(int id, LogLevel level, string message, string verbose, Exception exception = null)
        {
            Message?.Invoke(this, new ConsoleMessageEventArgs(id, DateTime.Now, level, message, IsVerbose ? verbose : null, exception));
        }
    }

    public class ConfigurableJsonRpcConsole : JsonRpcConsole<ServerConsoleOptions>
    {
        public ConfigurableJsonRpcConsole(ServerConsoleOptions configuration) 
            : base(configuration)
        {
        }
    }

    public abstract class JsonRpcConsole<TOptions> : JsonRpcConsole, IJsonRpcConsole<TOptions> 
        where TOptions : class, new()
    {
        protected JsonRpcConsole(TOptions configuration)
        {
            Configuration = configuration;
        }

        public TOptions Configuration { get; }
    }
}
