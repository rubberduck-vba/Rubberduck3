using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Platform.Model;
using Rubberduck.RPC.Proxy.SharedServices.Server.Configuration;
using Rubberduck.RPC.Proxy.SharedServices.Server.Model;
using StreamJsonRpc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.RPC.Proxy.SharedServices.Server.Abstract
{
    /// <summary>
    /// Handles server-level notifications and requests.
    /// </summary>
    /// <remarks>
    /// This interface must be implemented on the server side.
    /// Proxy implementations should be stateless: the instance only lives for the duration of a single request.
    /// </remarks>
    /// <typeparam name="TOptions">A type representing all server settings and capabilities.</typeparam>
    public interface IServerProxy<TOptions, TInitializeParams> : IConfigurableServerProxy<TOptions>, IJsonRpcTarget
        where TOptions : SharedServerCapabilities, new()
        where TInitializeParams : class, new()
    {
        /// <summary>
        /// An <c>Initialize</c> request is sent as the first request from a client to the server.
        /// If the server receives a request or notification before the <c>Initialize</c> request, it should:
        /// <list type="bullet">
        /// <item>For a <em>request</em>, respond with error code <c>-32002</c>.</item>
        /// <item>For a <em>notification</em> that isn't the <c>Exit</c> notification, the request should be dropped.</item>
        /// </list>
        /// </summary>
        /// <remarks>
        /// Implementation should allow server to <c>Exit</c> without receiving an <c>Initialize</c> request.
        /// Server may send <c>window/showMessage</c>, <c>window/showMessageRequest</c>, <c>window/logMessage</c>, 
        /// and <c>telemetry/event</c> requests to the client while the <c>Initialize</c> request is processing.
        /// </remarks>
        [JsonRpcMethod(JsonRpcMethods.ServerProxyRequests.Shared.Server.Initialize), LspCompliant]
        Task<InitializeResult<TOptions>> InitializeAsync(TInitializeParams parameters, CancellationToken token);

        /// <summary>
        /// Gets the current server info, including server uptime, process ID, connected clients, etc.
        /// </summary>
        [JsonRpcMethod(JsonRpcMethods.ServerProxyRequests.Shared.Server.Info)]
        Task<ServerState> RequestServerInfoAsync();

        /// <summary>
        /// Notifies any remaining listeners that the server is ready to terminate.
        /// </summary>
        event EventHandler WillExit;
        Task OnWillExitAsync();
    }
}