using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract(Name = "createFilesParams")]
    public class CreateFilesParams
    {
        /// <summary>
        /// An array of all files/folders created in this operation.
        /// </summary>
        [ProtoMember(1, Name = "files")]
        public FileCreate[] Files { get; set; }
    }
}
