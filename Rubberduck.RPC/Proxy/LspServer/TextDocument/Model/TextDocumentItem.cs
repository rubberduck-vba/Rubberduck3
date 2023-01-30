using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Model
{
    /// <summary>
    /// An item to transfer a text document from the client to the server.
    /// </summary>
    public class TextDocumentItem
    {
        /// <summary>
        /// The text document's URI.
        /// </summary>
        [JsonPropertyName("uri"), LspCompliant]
        public string Uri { get; set; }

        /// <summary>
        /// The text document's language identifier.
        /// </summary>
        [JsonPropertyName("languageId"), LspCompliant]
        public string LanguageId { get; set; } = "vb";

        /// <summary>
        /// The version number of this document (it will increase after each change, including undo/redo).
        /// </summary>
        [JsonPropertyName("version"), LspCompliant]
        public int Version { get; set; }

        /// <summary>
        /// The content of the opened text document.
        /// </summary>
        [JsonPropertyName("text"), LspCompliant]
        public string Text { get; set; }
    }
}
