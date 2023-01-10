using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Response
{
    public class SemanticTokensPartialResult
    {
        /// <summary>
        /// The actual tokens.
        /// </summary>
        [JsonPropertyName("data")]
        public uint[] Data { get; set; }
    }
}
