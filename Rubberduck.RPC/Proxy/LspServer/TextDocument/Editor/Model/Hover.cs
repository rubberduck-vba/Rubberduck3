using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.LspServer.TextDocument.Model;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Editor.Model
{
    /// <summary>
    /// The result of a hover request.
    /// </summary>
    public class Hover
    {
        /// <summary>
        /// The range this hover applies to.
        /// </summary>
        [JsonPropertyName("range"), LspCompliant]
        public Range Range { get; set; }

        /// <summary>
        /// The markdown content of the hover popup.
        /// </summary>
        [JsonPropertyName("content"), LspCompliant]
        public MarkupContent Content { get; set; }
    }
}
