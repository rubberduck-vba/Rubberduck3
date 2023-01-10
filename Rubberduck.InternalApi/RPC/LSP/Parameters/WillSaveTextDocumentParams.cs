using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract(Name = "willSaveTextDocumentParams")]
    public class WillSaveTextDocumentParams
    {
        /// <summary>
        /// The document that will be saved.
        /// </summary>
        [ProtoMember(1, Name = "textDocument")]
        public TextDocumentIdentifier TextDocument { get; set; }

        /// <summary>
        /// The <c>TextDocumentSaveReason</c> code.
        /// </summary>
        /// <remarks>
        /// See <c>Constants.TextDocumentSaveReason</c>.
        /// </remarks>
        [ProtoMember(2, Name = "reason")]
        public int Reason { get; set; }
    }
}
