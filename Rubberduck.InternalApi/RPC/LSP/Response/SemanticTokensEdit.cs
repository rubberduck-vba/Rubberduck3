using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Response
{
    public class SemanticTokensEdit
    {
        /// <summary>
        /// The start offset (zero-based) of the edit.
        /// </summary>
        [JsonPropertyName("start")]
        public uint Start { get; set; }

        /// <summary>
        /// The number of semantic tokens to remove.
        /// </summary>
        [JsonPropertyName("deleteCount")]
        public uint DeleteCount { get; set; }

        /// <summary>
        /// The semantic tokens to insert.
        /// </summary>
        [JsonPropertyName("data")]
        public uint[] Data { get; set; }
    }
}
