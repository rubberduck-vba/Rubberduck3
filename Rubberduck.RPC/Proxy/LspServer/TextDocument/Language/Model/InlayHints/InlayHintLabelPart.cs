using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.LspServer.TextDocument.Model;
using Rubberduck.RPC.Proxy.LspServer.Workspace.Model;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Model
{
    /// <summary>
    /// Inlay hint label part allows for interactive and composite labels for inlay hints.
    /// </summary>
    public class InlayHintLabelPart
    {
        /// <summary>
        /// The value of this label part.
        /// </summary>
        [JsonPropertyName("value"), LspCompliant]
        public string Value { get; set; }

        /// <summary>
        /// The tooltip text when you hover this label part.
        /// </summary>
        /// <remarks>
        /// Client may resolve this property late with a separate resolve request.
        /// </remarks>
        [JsonPropertyName("tooltip"), LspCompliant]
        public MarkupContent ToolTip { get; set; }

        /// <summary>
        /// An optional source code location representing this label part.
        /// </summary>
        [JsonPropertyName("location"), LspCompliant]
        public Location Location { get; set; }

        /// <summary>
        /// An optional command.
        /// </summary>
        [JsonPropertyName("command"), LspCompliant]
        public Command Command { get; set; }
    }
}
