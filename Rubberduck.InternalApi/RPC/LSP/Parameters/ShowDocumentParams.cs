using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    public class ShowDocumentParams
    {
        /// <summary>
        /// The document URI to show.
        /// </summary>
        [JsonPropertyName("uri")]
        public string Uri { get; set; }

        /// <summary>
        /// Whether to show the document in an external process, e.g. a new default browser window.
        /// </summary>
        [JsonPropertyName("external")]
        public bool External { get; set; }

        /// <summary>
        /// Whether the editor showing the document should get the focus.
        /// </summary>
        /// <remarks>
        /// Clients may ignore this property if the document is shown in an external process.
        /// </remarks>
        [JsonPropertyName("takeFocus")]
        public bool ShouldGetFocus { get; set; }

        /// <summary>
        /// An optional selection range, if the document is a text document.
        /// </summary>
        /// <remarks>
        /// Clients may ignore this property if the document is shown in an external process, or if the document is not a text file.
        /// </remarks>
        [JsonPropertyName("selection")]
        public Range Selection { get; set; }
    }
}
