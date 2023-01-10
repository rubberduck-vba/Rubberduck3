using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract(Name = "referencesParams")]
    public class ReferencesParams : TextDocumentPositionParams, IWorkDoneProgressParams, IPartialResultParams
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

        [ProtoMember(5, Name = "context")]
        public ReferenceContext Context { get; set; }

        [ProtoContract(Name = "referenceContext")]
        public class ReferenceContext
        {
            /// <summary>
            /// Whether the declaration of the symbol should be included in the results.
            /// </summary>
            [ProtoMember(1, Name = "includeDeclaration")]
            public bool IncludeDeclaration { get; set; }
        }
    }
}
