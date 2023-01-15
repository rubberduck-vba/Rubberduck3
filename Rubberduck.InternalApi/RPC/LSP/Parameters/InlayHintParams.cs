using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    public class InlayHintParams : WorkDoneProgressParams
    {
        /// <summary>
        /// The text document.
        /// </summary>
        [JsonPropertyName("textDocument")]
        public TextDocumentIdentifier TextDocument { get; set; }

        /// <summary>
        /// The visible document range for which inlay hints should be computed.
        /// </summary>
        [JsonPropertyName("range")]
        public Range Range { get; set; }
    }
}
