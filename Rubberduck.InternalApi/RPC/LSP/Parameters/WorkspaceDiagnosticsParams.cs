using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract(Name = "workspaceDiagnosticsParams")]
    public class WorkspaceDiagnosticsParams : WorkDoneProgressParams, IPartialResultParams
    {
        /// <summary>
        /// A token that the server can use to report partial results (e.g. streaming) to the client.
        /// </summary>
        [ProtoMember(2, Name = "partialResultToken")]
        public string PartialResultToken { get; set; }

        [ProtoMember(3, Name = "identifier")]
        public string Identifier { get; set; }

        /// <summary>
        /// The previous result IDs for the currently known diagnostic reports.
        /// </summary>
        [ProtoMember(4, Name = "previousResultIds")]
        public PreviousResultId[] PreviousResultIds { get; set; }
    }
}
