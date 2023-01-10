using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Response
{
    public class SemanticTokens : SemanticTokensPartialResult
    {
        /// <summary>
        /// An optional result ID. If provided, client may include the result ID in a subsequent semantic token request for delta-updating.
        /// </summary>
        [JsonPropertyName("resultId")]
        public string ResultId { get; set; }
    }
}
