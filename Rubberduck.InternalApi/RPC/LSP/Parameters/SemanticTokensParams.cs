using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract(Name = "semanticTokensParams")]
    public class SemanticTokensParams : WorkDoneProgressParams, IPartialResultParams
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
        /// The result ID of a previous response.
        /// </summary>
        /// <remarks>
        /// May point to a full or delta response, depending on what was received last.
        /// </remarks>
        [ProtoMember(4, Name = "previousResultId")]
        public string PreviousResultId { get; set; }
    }
}
