using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP
{
    /// <summary>
    /// Identifies a text document.
    /// </summary>
    public class TextDocumentIdentifier
    {
        /// <summary>
        /// The text document's URI.
        /// </summary>
        [JsonPropertyName("uri")]
        public string Uri { get; set; }
    }
}
