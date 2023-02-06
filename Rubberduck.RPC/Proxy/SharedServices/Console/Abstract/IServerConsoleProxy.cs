using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Proxy.SharedServices.Console.Commands.Parameters;
using Rubberduck.RPC.Proxy.SharedServices.Console.Configuration;
using Rubberduck.RPC.Proxy.SharedServices.Server.Configuration;
using System;
using System.Threading.Tasks;
using Rubberduck.RPC.Proxy.SharedServices.Abstract;

namespace Rubberduck.RPC.Proxy.SharedServices.Console.Abstract
{
    /// <summary>
    /// An interface that specifies server to client RPC communications.
    /// </summary>
    public interface IServerConsoleProxy : IJsonRpcTarget
    {
        /// <summary>
        /// A notification sent from the server to the client to log the trace of the server's execution.
        /// The amount and content of these notifications depends on the current <c>trace</c> configuration.
        /// </summary>
        /// <remarks>
        /// This notification should be used for systematic trace reporting.
        /// For single debugging messages, the server should send <c>window/logMessage</c> notifications.
        /// </remarks>
        [LspCompliant(JsonRpcMethods.ServerProxyRequests.Shared.Console.LogTrace)]

        event EventHandler<LogTraceParams> LogTrace;
        /// <summary>
        /// Sends a <c>LogTrace</c> notification to the client.
        /// </summary>
        Task OnLogTraceAsync(LogTraceParams logTraceParams);
    }

    /// <summary>
    /// Extends <see cref="IServerConsoleProxy"/> with server-side console functionality.
    /// </summary>
    /// <typeparam name="TOptions">The class type for console settings.</typeparam>
    public interface IServerConsoleProxy<TOptions> : IServerConsoleProxy, IConfigurableProxy<TOptions>
        where TOptions : SharedServerCapabilities, new()
    {
        /// <summary>
        /// Configuration options for the server console.
        /// </summary>
        ServerConsoleOptions Configuration { get; }

        /// <summary>
        /// Writes an error-level exception trace log message.
        /// </summary>
        /// <param name="exception">The exception to be logged.</param>
        Task LogTraceAsync(Exception exception);
        /// <summary>
        /// Writes an exception trace log at the specified level.
        /// </summary>
        /// <param name="exception">The exception to be logged.</param>
        /// <param name="level">The log level for this entry.</param>
        /// <param name="message">An optional message (<c>exception.Message</c> if unspecified).</param>
        /// <param name="verbose">An optional verbose message (<c>exception.StackTrace</c> if unspecified).</param>
        Task LogTraceAsync(Exception exception, ServerLogLevel level, string message = null, string verbose = null);

        /// <summary>
        /// Writes a trace log message at the specified level.
        /// </summary>
        /// <param name="level">The log level for this message.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="verbose">An optional verbose message with additional log data.</param>
        Task LogTraceAsync(ServerLogLevel level, string message, string verbose = null);
    }
}