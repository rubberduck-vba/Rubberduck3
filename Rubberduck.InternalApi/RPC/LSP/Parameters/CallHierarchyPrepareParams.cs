using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract(Name = "callHierarchyPrepareParams")]
    public class CallHierarchyPrepareParams : TextDocumentPositionParams, IWorkDoneProgressParams
    {
        /// <summary>
        /// A token that the server can use to report work done progress.
        /// </summary>
        [ProtoMember(3, Name = "workDoneToken")]
        public string WorkDoneToken { get; set; }
    }
}
