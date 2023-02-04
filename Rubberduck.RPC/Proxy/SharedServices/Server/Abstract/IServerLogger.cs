using System;

namespace Rubberduck.RPC.Proxy.SharedServices.Abstract
{
    /// <summary>
    /// A server-side logger service that outputs to the server console.
    /// </summary>
    public interface IServerLogger
    {
        /// <summary>
        /// Logs a trace message to the server trace output.
        /// </summary>
        /// <param name="message">The message content.</param>
        /// <param name="verbose">The verbose message content.</param>
        void OnTrace(string message, string verbose = null);
        /// <summary>
        /// Logs a debug message to the server trace output.
        /// </summary>
        /// <param name="message">The message content.</param>
        /// <param name="verbose">The verbose message content.</param>
        void OnDebug(string message, string verbose = null);
        /// <summary>
        /// Logs an information message to the server trace output.
        /// </summary>
        /// <param name="message">The message content.</param>
        /// <param name="verbose">The verbose message content.</param>
        void OnInfo(string message, string verbose = null);
        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">The message content.</param>
        /// <param name="verbose">The verbose message content.</param>
        void OnWarning(string message, string verbose = null);
        /// <summary>
        /// Logs an exception at error level to the server trace output.
        /// </summary>
        /// <param name="exception"></param>
        void OnError(Exception exception);
    }

    internal interface IInternalServerLogger : IServerLogger
    {
        /// <summary>
        /// Logs a message at the specified log level.
        /// </summary>
        /// <param name="level">The log level of the message.</param>
        /// <param name="message">The message content.</param>
        /// <param name="verbose">The verbose message content.</param>
        void Log(ServerLogLevel level, string message, string verbose = null);
    }
}