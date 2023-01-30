using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Model
{
    /// <summary>
    /// Identifies a text document.
    /// </summary>
    public class TextDocumentIdentifier
    {
        /// <summary>
        /// The text document's URI.
        /// </summary>
        [JsonPropertyName("uri"), LspCompliant]
        public string Uri { get; set; }
    }
}
