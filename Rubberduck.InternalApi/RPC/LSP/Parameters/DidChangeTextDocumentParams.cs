using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract(Name = "didChangeTextDocumentParams")]
    public class DidChangeTextDocumentParams
    {
        /// <summary>
        /// The document that was changed. The version number points to the version after all provided content changes have been applied.
        /// </summary>
        [ProtoMember(1, Name = "textDocument")]
        public VersionedTextDocumentIdentifier TextDocument { get; set; }

        [ProtoMember(2, Name = "contentChanges")]
        public TextDocumentContentChangeEvent[] ContentChanges { get; set; }
    }
}
