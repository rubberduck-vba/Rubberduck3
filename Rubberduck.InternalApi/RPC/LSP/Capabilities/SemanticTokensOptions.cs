using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    public class SemanticTokensOptions : WorkDoneProgressOptions
    {
        /// <summary>
        /// The legend used by the server.
        /// </summary>
        [JsonPropertyName("legend")]
        public SemanticTokenLegend Legend { get; set; }

        /// <summary>
        /// Whether the server supports providing semantic tokens for a specific range in a document.
        /// </summary>
        [JsonPropertyName("range")]
        public bool Range { get; set; }

        /// <summary>
        /// Whether the server supports providing semantic tokens for a full document.
        /// </summary>
        [JsonPropertyName("full")]
        public SupportsDelta Full { get; set; }

        public class SupportsDelta
        {
            /// <summary>
            /// If <c>true</c>, the server supports deltas for full documents.
            /// </summary>
            [JsonPropertyName("delta")]
            public bool Delta { get; set; }
        }
    }
}
