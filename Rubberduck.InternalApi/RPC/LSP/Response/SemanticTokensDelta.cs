using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Response
{
    public class SemanticTokensDelta
    {
        /// <remarks>
        /// LSP specifies this property as read-only.
        /// </remarks>
        [JsonPropertyName("resultId")]
        public string ResultId { get; set; }

        /// <summary>
        /// The semantic token edits to transform a previous result into the current state.
        /// </summary>
        [JsonPropertyName("edits")]
        public SemanticTokensEdit[] Edits { get; set; }
    }
}
