using System;

namespace Rubberduck.RPC.Proxy.SharedServices.Console.Configuration
{ 
    /// <summary>
    /// Represents a log message that is output to the server console.
    /// </summary>
    public class ConsoleMessage
    {
        /// <summary>
        /// A log message that is output to the server console.
        /// </summary>
        public ConsoleMessage(int id, DateTime timestamp, ServerLogLevel level, string message, string verbose, Exception exception = null)
        {
            Id = id;
            Timestamp = timestamp;

            Level = level;
            Message = message;
            Verbose = verbose;

            Exception = exception;
            IsError = exception != null;
        }

        /// <summary>
        /// A sequential message ID.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime Timestamp { get; }

        /// <summary>
        /// <c>true</c> if the <c>Exception</c> property contains error information.
        /// </summary>
        public bool IsError { get; }

        /// <summary>
        /// The message level. Client may use it to filter messages displayed.
        /// </summary>
        public ServerLogLevel Level { get; }

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

        public override string ToString() => $"{Id:000000} {Timestamp:yyyy-MM-dd hh:mm:ss.fff} {Level} {Message} {Verbose}".TrimEnd();
    }
}
