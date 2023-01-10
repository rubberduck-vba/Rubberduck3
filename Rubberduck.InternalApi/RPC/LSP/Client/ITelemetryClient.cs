using System.ServiceModel;
using System.Threading.Tasks;

namespace Rubberduck.InternalApi.RPC.LSP.Client
{
    /// <summary>
    /// Telemetry requests sent from a server to a client.
    /// </summary>
    [ServiceContract]
    public interface ITelemetryClient
    {
        /// <summary>
        /// A notification that prompts the client to log a telemetry event.
        /// </summary>
        /// <remarks>
        /// The protocol does not specify the payload.
        /// </remarks>
        [OperationContract(Name = "telemetry/event")]
        Task TelemetryEvent(object parameters);
    }
}
