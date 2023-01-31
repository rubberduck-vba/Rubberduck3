using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Platform.Model;
using Rubberduck.RPC.Proxy.SharedServices.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Console.Abstract;
using Rubberduck.RPC.Proxy.SharedServices.Console.Configuration;
using Rubberduck.RPC.Proxy.SharedServices.Server.Commands;
using Rubberduck.RPC.Proxy.SharedServices.Server.Model;
using Rubberduck.RPC.Proxy.SharedServices.Telemetry;
using StreamJsonRpc;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.RPC.Proxy.SharedServices.Server.Abstract
{
    /// <summary>
    /// Handles server-level notifications and requests sent from the client.
    /// </summary>
    /// <remarks>
    /// This interface must be implemented on the server side.
    /// Proxy implementations should be stateless: the instance only lives for the duration of a single request.
    /// </remarks>
    /// <typeparam name="TOptions">A type representing all server settings and capabilities.</typeparam>
    public interface IServerProxyService<TOptions, TClientProxy> : IConfigurableServerProxy<TOptions, TClientProxy>, IServerCommandsProxy<TOptions>
        where TOptions : class, new()
        where TClientProxy : class
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
        Task<InitializeResult<TOptions>> InitializeAsync(InitializeParams<TOptions> parameters, CancellationToken token);

        /// <summary>
        /// Gets the current server info, including server uptime, process ID, connected clients, etc.
        /// </summary>
        [JsonRpcMethod(JsonRpcMethods.ServerProxyRequests.Shared.Server.Info)]
        ServerState Info();

        /// <summary>
        /// Exposes server console services and configurations.
        /// </summary>
        IServerConsoleService<ServerConsoleOptions> ServerConsole { get; }

        /// <summary>
        /// Exposes server telemetry services and configurations.
        /// </summary>
        ITelemetryClientService Telemetry { get; }
    }
}