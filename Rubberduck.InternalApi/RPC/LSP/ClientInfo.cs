using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP
{
    public class ClientInfo
    {
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
