using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    public class PreviousResultId
    {
        [JsonPropertyName("uri")]
        public string DocumentUri { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
}
