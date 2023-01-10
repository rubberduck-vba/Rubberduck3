using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract(Name = "documentColorParams")]
    public class DocumentColorParams : WorkDoneProgressParams, IPartialResultParams
    {
        /// <summary>
        /// A token that the server can use to report partial results (e.g. streaming) to the client.
        /// </summary>
        [ProtoMember(2, Name = "partialResultToken")]
        public string PartialResultToken { get; set; }

        [ProtoMember(3, Name = "textDocument")]
        public TextDocumentIdentifier TextDocument { get; set; }
    }
}
