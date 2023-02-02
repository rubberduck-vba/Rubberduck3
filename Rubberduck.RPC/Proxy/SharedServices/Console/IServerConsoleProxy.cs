using System;
using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.SharedServices.Console.Commands.Parameters;

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
    }
}
