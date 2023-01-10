using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract(Name = "documentFormattingParams")]
    public class DocumentFormattingParams : WorkDoneProgressParams
    {
        /// <summary>
        /// The document to format.
        /// </summary>
        [ProtoMember(2, Name = "textDocument")]
        public TextDocumentIdentifier TextDocument { get; set; }

        /// <summary>
        /// The formatting options.
        /// </summary>
        [ProtoMember(3, Name = "options")]
        public FormattingOptions Options { get; set; }
    }

    [ProtoContract(Name = "rangeFormattingParams")]
    public class RangeFormattingParams : DocumentFormattingParams
    {
        /// <summary>
        /// The range to format.
        /// </summary>
        [ProtoMember(4, Name = "range")]
        public Range Range { get; set; }
    }
}
