using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC
{
    public class ClientInfo
    {
        /// <summary>
        /// The PID of the client.
        /// </summary>
        [JsonPropertyName("processId")]
        public int ProcessId { get; set; }

        /// <summary>
        /// The name of the client, as defined by the client.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The client's version, as defined by the client.
        /// </summary>
        [JsonPropertyName("version")]
        public string Version { get; set; }
    }
}
