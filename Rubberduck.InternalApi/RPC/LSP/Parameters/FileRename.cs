using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract(Name = "fileRename")]
    public class FileRename
    {
        /// <summary>
        /// The old/current URI for the location of the file/folder being renamed.
        /// </summary>
        [ProtoMember(1, Name = "oldUri")]
        public string OldUri { get; set; }

        /// <summary>
        /// The URI of the new location of the file/folder being renamed.
        /// </summary>
        [ProtoMember(2, Name = "newUri")]
        public string NewUri { get; set; }
    }
}
