using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    /// <summary>
    /// Encodes token types and modifiers by index.
    /// </summary>
    public class SemanticTokenLegend
    {
        /// <summary>
        /// The token types a server uses.
        /// </summary>
        [JsonPropertyName("tokenTypes")]
        public string[] TokenTypes { get; set; }
        /// <summary>
        /// The token modifiers a server uses.
        /// </summary>
        [JsonPropertyName("tokenModifiers")]
        public string[] TokenModifiers { get; set; }
    }
}
