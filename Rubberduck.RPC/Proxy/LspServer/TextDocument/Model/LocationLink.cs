using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Model
{
    /// <summary>
    /// Represents a link between a source and a target location.
    /// </summary>
    public class LocationLink
    {
        /// <summary>
        /// Span of the origin of this link.
        /// </summary>
        /// <remarks>
        /// Used as the underlined span for mouse interaction. Defaults to the word range at the mouse position.
        /// </remarks>
        [JsonPropertyName("originSelectionRange"), LspCompliant]
        public Range OriginSelectionRange { get; set; }

        /// <summary>
        /// The target resource identifier for this link.
        /// </summary>
        [JsonPropertyName("targetUri"), LspCompliant]
        public string TargetUri { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("targetRange"), LspCompliant]
        public Range TargetRange { get; set; }

        /// <summary>
        /// The range that should be selected and revealed when this link is being followed, e.g. the name of a function.
        /// Must be contained by the TargetRange.
        /// </summary>
        [JsonPropertyName("targetSelectionRange"), LspCompliant]
        public Range TargetSelectionRange { get; set; }
    }
}
