using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.SharedServices.Server.Responses
{
    public class DisconnectResult
    {
        /// <summary>
        /// Whether the client was successfully disconnected.
        /// </summary>
        [JsonPropertyName("disconnected")]
        public bool Disconnected { get; set; }

        /// <summary>
        /// Whether the server is shutting down.
        /// </summary>
        [JsonPropertyName("shuttingDown")]
        public bool ShuttingDown { get; set; }
    }
}
