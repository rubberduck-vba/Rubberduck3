using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract(Name = "didCloseTextDocumentParams")]
    public class DidCloseTextDocumentParams
    {
        /// <summary>
        /// The document that was closed.
        /// </summary>
        [ProtoMember(1, Name ="textDocument")]
        public TextDocumentIdentifier TextDocument { get; set; }
    }
}
