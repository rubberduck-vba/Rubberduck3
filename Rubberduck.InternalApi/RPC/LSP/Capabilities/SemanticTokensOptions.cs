using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "semanticTokensOptions")]
    public class SemanticTokensOptions : WorkDoneProgressOptions
    {
        /// <summary>
        /// The legend used by the server.
        /// </summary>
        [ProtoMember(2, Name = "legend")]
        public SemanticTokenLegend Legend { get; set; }

        /// <summary>
        /// Whether the server supports providing semantic tokens for a specific range in a document.
        /// </summary>
        [ProtoMember(3, Name = "range")]
        public bool Range { get; set; }

        /// <summary>
        /// Whether the server supports providing semantic tokens for a full document.
        /// </summary>
        [ProtoMember(4, Name = "full")]
        public SupportsDelta Full { get; set; }

        [ProtoContract(Name =  "supportsDelta")]
        public class SupportsDelta
        {
            /// <summary>
            /// If <c>true</c>, the server supports deltas for full documents.
            /// </summary>
            [ProtoMember(1, Name = "delta")]
            public bool Delta { get; set; }
        }
    }
}
