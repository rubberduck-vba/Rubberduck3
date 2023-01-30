using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.SharedServices.WorkDoneProgress.Configuration;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Configuration.Options
{
    public class SemanticTokensOptions : WorkDoneProgressOptions
    {
        /// <summary>
        /// The legend used by the server.
        /// </summary>
        [JsonPropertyName("legend"), LspCompliant]
        public SemanticTokenLegend Legend { get; set; }

        /// <summary>
        /// Whether the server supports providing semantic tokens for a specific range in a document.
        /// </summary>
        [JsonPropertyName("range"), LspCompliant]
        public bool Range { get; set; }

        /// <summary>
        /// Whether the server supports providing semantic tokens for a full document.
        /// </summary>
        [JsonPropertyName("full"), LspCompliant]
        public SupportsDelta Full { get; set; }

        public class SupportsDelta
        {
            /// <summary>
            /// If <c>true</c>, the server supports deltas for full documents.
            /// </summary>
            [JsonPropertyName("delta"), LspCompliant]
            public bool Delta { get; set; }
        }
    }
}
