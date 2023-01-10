using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract(Name = "didOpenTextDocumentParams")]
    public class DidOpenTextDocumentParams
    {
        /// <summary>
        /// The document that was opened.
        /// </summary>
        [ProtoMember(1, Name = "textDocument")]
        public TextDocumentItem TextDocument { get; set; }
    }
}
