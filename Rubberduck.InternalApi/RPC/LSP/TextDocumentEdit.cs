using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP
{
    /// <summary>
    /// Describes textual changes on a single text document.
    /// </summary>
    public class TextDocumentEdit
    {
        /// <summary>
        /// The text document to change.
        /// </summary>
        [JsonPropertyName("textDocument")]
        public OptionalVersionedTextDocumentIdentifier TextDocument { get; set; }

        /// <summary>
        /// The edits to be applied.
        /// </summary>
        /// <remarks>
        /// Edits can also be IAnnotatedTextEdit.
        /// </remarks>
        [JsonPropertyName("edits")]
        public TextEdit[] Edits { get; set; }
    }
}
