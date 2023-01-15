using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    public class DocumentOnTypeFormattingParams
    {
        /// <summary>
        /// The document to format.
        /// </summary>
        [JsonPropertyName("textDocument")]
        public TextDocumentIdentifier TextDocument { get; set; }

        [JsonPropertyName("position")]
        public Position Position { get; set; }

        [JsonPropertyName("ch")]
        public string Ch { get; set; }

        [JsonPropertyName("options")]
        public FormattingOptions Options { get; set; }
    }
}
