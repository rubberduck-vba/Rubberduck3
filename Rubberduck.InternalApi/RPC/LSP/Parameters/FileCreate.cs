using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract(Name = "fileCreate")]
    public class FileCreate
    {
        /// <summary>
        /// A URI for the location of the file/folder being created.
        /// </summary>
        [ProtoMember(1, Name = "uri")]
        public string Uri { get; set; }
    }
}
