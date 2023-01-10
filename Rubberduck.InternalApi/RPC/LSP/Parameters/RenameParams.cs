using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract(Name = "renameParams")]
    public class RenameParams : TextDocumentPositionParams, IWorkDoneProgressParams
    {
        /// <summary>
        /// A token that the server can use to report work done progress.
        /// </summary>
        [ProtoMember(3, Name = "workDoneToken")]
        public string WorkDoneToken { get; set; }

        /// <summary>
        /// The new name of the symbol. If the given name is invalid the request must return a ResponseError with an appropriate message set.
        /// </summary>
        [ProtoMember(4, Name = "newName")]
        public string NewName { get; set; }
    }
}
