using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Response
{
    public class DocumentLink
    {
        /// <summary>
        /// The range this link applies to.
        /// </summary>
        [JsonPropertyName("range")]
        public Range Range { get; set; }

        /// <summary>
        /// The URI this link points to. A resolve request can be sent later if missing.
        /// </summary>
        [JsonPropertyName("target")]
        public string Target { get; set; }

        /// <summary>
        /// The tooltip text when you hover this link.
        /// </summary>
        [JsonPropertyName("tooltip")]
        public string ToolTip { get; set; }

        /// <summary>
        /// A data entry field that is preserved on a document link between a DocumentLinkRequest and a DocumentLinkResolveRequest.
        /// </summary>
        [JsonPropertyName("data")]
        public LSPAny Data { get; set; }
    }
}
