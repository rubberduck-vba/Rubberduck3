using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.LspServer.TextDocument.Model;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Window.Commands.Parameters
{
    public class ShowDocumentParams
    {
        /// <summary>
        /// The document URI to show.
        /// </summary>
        [JsonPropertyName("uri"), LspCompliant]
        public string Uri { get; set; }

        /// <summary>
        /// Whether to show the document in an external process, e.g. a new default browser window.
        /// </summary>
        [JsonPropertyName("external"), LspCompliant]
        public bool External { get; set; }

        /// <summary>
        /// Whether the editor showing the document should get the focus.
        /// </summary>
        /// <remarks>
        /// Clients may ignore this property if the document is shown in an external process.
        /// </remarks>
        [JsonPropertyName("takeFocus"), LspCompliant]
        public bool ShouldGetFocus { get; set; }

        /// <summary>
        /// An optional selection range, if the document is a text document.
        /// </summary>
        /// <remarks>
        /// Clients may ignore this property if the document is shown in an external process, or if the document is not a text file.
        /// </remarks>
        [JsonPropertyName("selection"), LspCompliant]
        public Range Selection { get; set; }
    }
}
