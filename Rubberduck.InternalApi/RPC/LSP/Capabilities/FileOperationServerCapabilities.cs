using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "fileOperationServerCapabilities")]
    public class FileOperationServerCapabilities
    {
        [ProtoMember(1, Name = "didCreate")]
        public FileOperationRegistrationOptions DidCreate { get; set; }

        [ProtoMember(2, Name = "willCreate")]
        public FileOperationRegistrationOptions WillCreate { get; set; }

        [ProtoMember(3, Name = "didRename")]
        public FileOperationRegistrationOptions DidRename { get; set; }

        [ProtoMember(4, Name = "willRename")]
        public FileOperationRegistrationOptions WillRename { get; set; }

        [ProtoMember(5, Name = "didDelete")]
        public FileOperationRegistrationOptions DidDelete { get; set; }

        [ProtoMember(6, Name = "willDelete")]
        public FileOperationRegistrationOptions WillDelete { get; set; }
    }
}
