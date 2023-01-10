using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract(Name = "completionParams")]
    public class CompletionParams : TextDocumentPositionParams, IWorkDoneProgressParams, IPartialResultParams
    {
        /// <summary>
        /// A token that the server can use to report partial results (e.g. streaming) to the client.
        /// </summary>
        [ProtoMember(3, Name = "partialResultToken")]
        public string PartialResultToken { get; set; }

        /// <summary>
        /// A token that the server can use to report work done progress.
        /// </summary>
        [ProtoMember(4, Name = "workDoneToken")]
        public string WorkDoneToken { get; set; }

        /// <summary>
        /// The completion context, if the client signaled support for it.
        /// </summary>
        [ProtoMember(5, Name = "context")]
        public CompletionContext Context { get; set; }
    }
}
