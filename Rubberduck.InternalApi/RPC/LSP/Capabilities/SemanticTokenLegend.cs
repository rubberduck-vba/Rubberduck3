using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    /// <summary>
    /// Encodes token types and modifiers by index.
    /// </summary>
    [ProtoContract(Name = "semanticTokenLegend")]
    public class SemanticTokenLegend
    {
        /// <summary>
        /// The token types a server uses.
        /// </summary>
        [ProtoMember(1, Name = "tokenTypes")]
        public string[] TokenTypes { get; set; }
        /// <summary>
        /// The token modifiers a server uses.
        /// </summary>
        [ProtoMember(2, Name = "tokenModifiers")]
        public string[] TokenModifiers { get; set; }
    }
}
