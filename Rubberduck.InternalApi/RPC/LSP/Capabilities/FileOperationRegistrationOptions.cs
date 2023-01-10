using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "fileOperationRegistrationOptions")]
    public class FileOperationRegistrationOptions
    {
        [ProtoMember(1, Name = "filters")]
        public FileOperationFilter[] Filters { get; set; }
    }
}
