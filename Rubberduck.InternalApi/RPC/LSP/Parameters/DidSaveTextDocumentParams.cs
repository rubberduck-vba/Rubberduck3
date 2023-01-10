using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract(Name = "didSaveTextDocumentParams")]
    public class DidSaveTextDocumentParams
    {
        /// <summary>
        /// The document that was saved.
        /// </summary>
        [ProtoMember(1, Name = "textDocument")]
        public TextDocumentIdentifier TextDocument { get; set; }

        /// <summary>
        /// The saved content. <c>null</c> unless <c>IncludeText</c> was <c>true</c> when the save notification was requested.
        /// </summary>
        [ProtoMember(2, Name = "text")]
        public string Text { get; set; }
    }
}
