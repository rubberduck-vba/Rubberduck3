using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Model
{
    /// <summary>
    /// Describes textual changes on a single text document.
    /// </summary>
    public class TextDocumentEdit
    {
        /// <summary>
        /// The text document to change.
        /// </summary>
        [JsonPropertyName("textDocument"), LspCompliant]
        public OptionalVersionedTextDocumentIdentifier TextDocument { get; set; }

        /// <summary>
        /// The edits to be applied.
        /// </summary>
        /// <remarks>
        /// Edits can also be IAnnotatedTextEdit.
        /// </remarks>
        [JsonPropertyName("edits"), LspCompliant]
        public TextEdit[] Edits { get; set; }
    }
}
