using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "fileOperationPatternOptions")]
    public class FileOperationPatternOptions
    {
        [ProtoMember(1, Name = "ignoreCase")]
        public bool IgnoreCase { get; set; }
    }
}
