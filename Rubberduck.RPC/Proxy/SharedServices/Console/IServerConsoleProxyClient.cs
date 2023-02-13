using System;
using System.Threading.Tasks;
using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.SharedServices.Console.Commands.Parameters;

namespace Rubberduck.RPC.Proxy.SharedServices.Console.Abstract
{
    [JsonRpcSource]
    public interface IServerConsoleProxyClient : IJsonRpcSource
    {
        Task LogMessageAsync(LogMessageParams parameter);

        /// <summary>
        /// A notification sent from the client to the server to modify the server's trace level.
        /// </summary>
        [LspCompliant(JsonRpcMethods.ServerProxyRequests.Shared.Console.SetTrace)]
        event EventHandler<SetTraceParams> SetTrace;
        /// <summary>
        /// Sents a <c>$/setTrace</c> request to the server.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        Task OnSetTraceAsync(SetTraceParams parameter);

        /// <summary>
        /// A notification sent from the client to the server to stop/pause trace logging.
        /// </summary>
        [LspCompliant(JsonRpcMethods.ServerProxyRequests.Shared.Console.StopTrace)]
        event EventHandler StopTrace;
        /// <summary>
        /// Sents a <c>$/stopTrace</c> request to the server.
        /// </summary>
        Task OnStopTraceAsync();

        /// <summary>
        /// A notification sent from the client to the server to resume trace logging.
        /// </summary>
        [LspCompliant(JsonRpcMethods.ServerProxyRequests.Shared.Console.ResumeTrace)]
        event EventHandler ResumeTrace;
        /// <summary>
        /// Sents a <c>$/resumeTrace</c> request to the server.
        /// </summary>
        Task OnResumeTraceAsync();
    }
}
