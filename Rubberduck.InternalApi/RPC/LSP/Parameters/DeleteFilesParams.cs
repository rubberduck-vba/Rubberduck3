using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract(Name = "deleteFilesParams")]
    public class DeleteFilesParams
    {
        /// <summary>
        /// The files/folders deleted in this operation.
        /// </summary>
        [ProtoMember(1, Name = "files")]
        public FileDelete[] Files { get; set; }
    }
}
