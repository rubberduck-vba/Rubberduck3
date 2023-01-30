using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Model
{
    public class SemanticTokensDelta
    {
        /// <remarks>
        /// LSP specifies this property as read-only.
        /// </remarks>
        [JsonPropertyName("resultId"), LspCompliant]
        public string ResultId { get; set; }

        /// <summary>
        /// The semantic token edits to transform a previous result into the current state.
        /// </summary>
        [JsonPropertyName("edits"), LspCompliant]
        public SemanticTokensEdit[] Edits { get; set; }
    }
}
