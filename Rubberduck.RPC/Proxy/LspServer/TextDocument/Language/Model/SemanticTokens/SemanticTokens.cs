using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Model
{
    public class SemanticTokens : SemanticTokensPartialResult
    {
        /// <summary>
        /// An optional result ID. If provided, client may include the result ID in a subsequent semantic token request for delta-updating.
        /// </summary>
        [JsonPropertyName("resultId"), LspCompliant]
        public string ResultId { get; set; }
    }
}
