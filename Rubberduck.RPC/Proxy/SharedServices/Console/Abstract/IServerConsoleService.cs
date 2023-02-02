using Rubberduck.RPC.Proxy.SharedServices.Console.Commands;
using Rubberduck.RPC.Proxy.SharedServices.Console.Configuration;
using Rubberduck.RPC.Proxy.SharedServices.Server.Configuration;
using System;

namespace Rubberduck.RPC.Proxy.SharedServices.Console.Abstract
{
    /// <summary>
    /// Extends <see cref="IServerConsoleProxy"/> with server-side console functionality.
    /// </summary>
    /// <typeparam name="TOptions">The class type for console settings.</typeparam>
    public interface IServerConsoleService<TOptions> : IServerConsoleProxy
        where TOptions : SharedServerCapabilities
    {
        /// <summary>
        /// Configuration options for the server console.
        /// </summary>
        TOptions Configuration { get; }

        /// <summary>
        /// Exposes server console commands.
        /// </summary>
        ServerConsoleCommands Commands { get; }

        /// <summary>
        /// A <c>Message</c> notification outputs the provided <c>ConsoleMessage</c>.
        /// </summary>
        event EventHandler<ConsoleMessage> Message;

        /// <summary>
        /// Writes an error-level exception log.
        /// </summary>
        /// <param name="exception">The exception to be logged.</param>
        void Log(Exception exception);
        /// <summary>
        /// Writes an exception log at the specified level.
        /// </summary>
        /// <param name="exception">The exception to be logged.</param>
        /// <param name="level">The log level for this entry.</param>
        /// <param name="message">An optional message (<c>exception.Message</c> if unspecified).</param>
        /// <param name="verbose">An optional verbose message (<c>exception.StackTrace</c> if unspecified).</param>
        void Log(Exception exception, ServerLogLevel level, string message = null, string verbose = null);

        /// <summary>
        /// Writes a message log at the specified level.
        /// </summary>
        /// <param name="level">The log level for this message.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="verbose">An optional verbose message with additional log data.</param>
        void Log(ServerLogLevel level, string message, string verbose = null);
    }
}