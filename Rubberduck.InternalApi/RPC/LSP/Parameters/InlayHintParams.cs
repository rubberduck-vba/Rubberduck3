using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract(Name = "inlayHintParams")]
    public class InlayHintParams : WorkDoneProgressParams
    {
        /// <summary>
        /// The text document.
        /// </summary>
        [ProtoMember(2, Name = "textDocument")]
        public TextDocumentIdentifier TextDocument { get; set; }

        /// <summary>
        /// The visible document range for which inlay hints should be computed.
        /// </summary>
        [ProtoMember(3, Name = "range")]
        public Range Range { get; set; }
    }
}
