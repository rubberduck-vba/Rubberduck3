using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Model
{
    /// <summary>
    /// A textual edit applicable to a text document.
    /// </summary>
    public class TextEdit
    {
        /// <summary>
        /// The range of the text document to be manipulated. To insert text into a document create a range where start == end.
        /// </summary>
        [JsonPropertyName("range"), LspCompliant]
        public Range Range { get; set; }

        /// <summary>
        /// The string to be inserted. For delete operations use an empty string.
        /// </summary>
        [JsonPropertyName("newText"), LspCompliant]
        public string NewText { get; set; }
    }
}
