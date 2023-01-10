using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract(Name = "renameFilesParams")]
    public class RenameFilesParams
    {
        /// <summary>
        /// The files/folders renamed in this operation.
        /// </summary>
        [ProtoMember(1, Name = "files")]
        public FileRename[] Files { get; set; }
    }
}
