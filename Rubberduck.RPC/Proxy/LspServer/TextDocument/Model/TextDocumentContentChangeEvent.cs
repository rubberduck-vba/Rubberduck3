using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Model
{
    /// <summary>
    /// An event describing a change to a text document.
    /// </summary>
    public class TextDocumentContentChangeEvent
    {
        /// <summary>
        /// The range of the document that changed.
        /// </summary>
        [JsonPropertyName("range"), LspCompliant]
        public Range Range { get; set; }

        /// <summary>
        /// The new text for the provided range.
        /// </summary>
        /// <remarks>
        /// If <c>Range</c> is <c>null</c>, contains the text for the full document.
        /// </remarks>
        [JsonPropertyName("text"), LspCompliant]
        public string Text { get; set; }
    }
}
