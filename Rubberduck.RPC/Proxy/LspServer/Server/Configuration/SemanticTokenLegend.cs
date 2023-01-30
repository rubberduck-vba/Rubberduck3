using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Configuration
{
    /// <summary>
    /// Encodes token types and modifiers by index.
    /// </summary>
    public class SemanticTokenLegend
    {
        /// <summary>
        /// The token types a server uses.
        /// </summary>
        [JsonPropertyName("tokenTypes"), LspCompliant]
        public string[] TokenTypes { get; set; }
        /// <summary>
        /// The token modifiers a server uses.
        /// </summary>
        [JsonPropertyName("tokenModifiers"), LspCompliant]
        public string[] TokenModifiers { get; set; }
    }
}
