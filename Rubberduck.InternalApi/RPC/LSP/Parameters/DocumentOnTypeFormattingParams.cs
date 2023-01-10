using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract(Name = "documentOnTypeFormattingParams")]
    public class DocumentOnTypeFormattingParams
    {
        /// <summary>
        /// The document to format.
        /// </summary>
        [ProtoMember(1, Name = "textDocument")]
        public TextDocumentIdentifier TextDocument { get; set; }

        [ProtoMember(2, Name = "position")]
        public Position Position { get; set; }

        [ProtoMember(3, Name = "ch")]
        public string Ch { get; set; }

        [ProtoMember(4, Name = "options")]
        public FormattingOptions Options { get; set; }
    }
}
