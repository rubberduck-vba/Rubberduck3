using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Model
{
    public class SemanticTokensPartialResult
    {
        /// <summary>
        /// The actual tokens.
        /// </summary>
        [JsonPropertyName("data"), LspCompliant]
        public uint[] Data { get; set; }
    }
}
