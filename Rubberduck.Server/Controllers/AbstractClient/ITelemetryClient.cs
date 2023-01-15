using AustinHarris.JsonRpc;
using Rubberduck.InternalApi.RPC.LSP;
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
        [JsonRpcMethod(JsonRpcMethods.TelemetryEvent)]
        Task TelemetryEvent(object parameters);
    }
}
