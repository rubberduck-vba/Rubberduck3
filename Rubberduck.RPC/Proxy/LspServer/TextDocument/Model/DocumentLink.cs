using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Model
{
    public class DocumentLink
    {
        /// <summary>
        /// The range this link applies to.
        /// </summary>
        [JsonPropertyName("range"), LspCompliant]
        public Range Range { get; set; }

        /// <summary>
        /// The URI this link points to. A resolve request can be sent later if missing.
        /// </summary>
        [JsonPropertyName("target"), LspCompliant]
        public string Target { get; set; }

        /// <summary>
        /// The tooltip text when you hover this link.
        /// </summary>
        [JsonPropertyName("tooltip"), LspCompliant]
        public string ToolTip { get; set; }

        /// <summary>
        /// A data entry field that is preserved on a document link between a DocumentLinkRequest and a DocumentLinkResolveRequest.
        /// </summary>
        [JsonPropertyName("data"), LspCompliant]
        public object Data { get; set; }
    }
}
