using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "fileOperationFilter")]
    public class FileOperationFilter
    {
        [ProtoMember(1, Name = "scheme")]
        public string Scheme { get; set; }

        [ProtoMember(2, Name = "pattern")]
        public FileOperationPattern Pattern { get; set; }
    }
}
