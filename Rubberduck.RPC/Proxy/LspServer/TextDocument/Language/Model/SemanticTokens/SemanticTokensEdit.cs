using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Model
{
    public class SemanticTokensEdit
    {
        /// <summary>
        /// The start offset (zero-based) of the edit.
        /// </summary>
        [JsonPropertyName("start"), LspCompliant]
        public uint Start { get; set; }

        /// <summary>
        /// The number of semantic tokens to remove.
        /// </summary>
        [JsonPropertyName("deleteCount"), LspCompliant]
        public uint DeleteCount { get; set; }

        /// <summary>
        /// The semantic tokens to insert.
        /// </summary>
        [JsonPropertyName("data"), LspCompliant]
        public uint[] Data { get; set; }
    }
}
