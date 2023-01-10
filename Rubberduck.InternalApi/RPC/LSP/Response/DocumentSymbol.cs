using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Response
{
    public class DocumentSymbol
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("detail")]
        public string Details { get; set; }

        [JsonPropertyName("kind")]
        public int SymbolKind { get; set; }

        [JsonPropertyName("tags")]
        public int[] SymbolTags { get; set; }

        [JsonPropertyName("range")]
        public Range Range { get; set; }

        [JsonPropertyName("selectionRange")]
        public Range SelectionRange { get; set; }

        [JsonPropertyName("children")]
        public DocumentSymbol[] Children { get; set; }
    }
}
