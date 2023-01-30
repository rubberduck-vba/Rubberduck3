using AustinHarris.JsonRpc;
using Rubberduck.RPC.Platform.Metadata;
using System.Threading.Tasks;

namespace Rubberduck.Server.LSP.Controllers.AbstractClient
{
    /// <summary>
    /// Telemetry requests sent from a server to a client.
    /// </summary>
    public interface ITelemetryClient
    {
        /// <summary>
        /// A notification that prompts the client to log a telemetry event.
        /// </summary>
        /// <remarks>
        /// The protocol does not specify the payload.
        /// </remarks>
        [JsonRpcMethod(JsonRpcMethods.ClientProxyRequests.Telemetry.TelemetryEvent), LspCompliant]
        Task TelemetryEventAsync(object parameters);
    }
}
