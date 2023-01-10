using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Response
{
    public class CompletionList
    {
        [JsonPropertyName("isIncomplete")]
        public bool IsIncomplete { get; set; }

        [JsonPropertyName("items")]
        public CompletionItem[] Items { get; set; }
    }
}
