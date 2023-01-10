using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract(Name = "selectionRangeParams")]
    public class SelectionRangeParams : WorkDoneProgressParams, IPartialResultParams
    {
        /// <summary>
        /// A token that the server can use to report partial results (e.g. streaming) to the client.
        /// </summary>
        [ProtoMember(2, Name = "partialResultToken")]
        public string PartialResultToken { get; set; }

        /// <summary>
        /// The text document.
        /// </summary>
        [ProtoMember(3, Name = "textDocument")]
        public TextDocumentIdentifier TextDocument { get; set; }

        /// <summary>
        /// The positions inside the text document.
        /// </summary>
        [ProtoMember(4, Name = "positions")]
        public Position[] Positions { get; set; }
    }
}
