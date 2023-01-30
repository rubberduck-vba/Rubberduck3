using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.SharedServices.Server.Commands
{
    public class ClientShutdownParams
    {
        /// <summary>
        /// Identifies the client that is sending a shutdown notification to the server.
        /// </summary>
        [JsonPropertyName("processId")]
        public int ProcessId { get; set; }
    }
}
