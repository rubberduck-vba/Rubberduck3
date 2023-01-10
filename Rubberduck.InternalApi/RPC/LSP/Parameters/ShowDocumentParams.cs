using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract(Name = "showDocumentParams")]
    public class ShowDocumentParams
    {
        /// <summary>
        /// The document URI to show.
        /// </summary>
        [ProtoMember(1, Name = "uri")]
        public string Uri { get; set; }

        /// <summary>
        /// Whether to show the document in an external process, e.g. a new default browser window.
        /// </summary>
        [ProtoMember(2, Name = "external")]
        public bool External { get; set; }

        /// <summary>
        /// Whether the editor showing the document should get the focus.
        /// </summary>
        /// <remarks>
        /// Clients may ignore this property if the document is shown in an external process.
        /// </remarks>
        [ProtoMember(3, Name = "takeFocus")]
        public bool TakeFocus { get; set; }

        /// <summary>
        /// An optional selection range, if the document is a text document.
        /// </summary>
        /// <remarks>
        /// Clients may ignore this property if the document is shown in an external process, or if the document is not a text file.
        /// </remarks>
        [ProtoMember(4, Name = "selection")]
        public Range Selection { get; set; }
    }
}
