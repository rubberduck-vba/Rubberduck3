using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "regularExpressionClientCapabilities")]
    public class RegularExpressionClientCapabilies
    {
        [ProtoMember(1, Name = "engine")]
        public string Engine { get; set; }

        [ProtoMember(2, Name = "version")]
        public string Version { get; set; }
    }
}
