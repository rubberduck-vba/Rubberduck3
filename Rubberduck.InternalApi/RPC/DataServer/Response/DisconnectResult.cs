using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.DataServer.Response
{
    public class DisconnectResult
    {
        [JsonPropertyName("disconnected")]
        public bool Disconnected { get; set; }
        [JsonPropertyName("shuttingDown")]
        public bool ShuttingDown { get; set; }
    }
}
