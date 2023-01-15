using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    public class LogMessageParams
    {
        [JsonPropertyName("messageType")]
        public int MessageType { get; set; } = Constants.MessageType.Log;

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
