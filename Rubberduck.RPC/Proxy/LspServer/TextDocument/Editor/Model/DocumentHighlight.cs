using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.LspServer.TextDocument.Model;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Editor.Model
{
    /// <summary>
    /// A range inside a text document which deserves special attention.
    /// </summary>
    public class DocumentHighlight
    {
        /// <summary>
        /// The range this highlight applies to.
        /// </summary>
        [JsonPropertyName("range"), LspCompliant]
        public Range Range { get; set; }

        /// <summary>
        /// The kind of highlight. See <c>Constants.DocumentHighlightKind</c>.
        /// </summary>
        [JsonPropertyName("kind"), LspCompliant]
        public Constants.DocumentHighlightKind.AsEnum Kind { get; set; }
    }
}
