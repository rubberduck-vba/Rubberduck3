using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    public class ShowMessageParams
    {
        [JsonPropertyName("type")]
        public int MessageType { get; set; } = Constants.MessageType.Info;

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
