using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract(Name = "previousResultId")]
    public class PreviousResultId
    {
        [ProtoMember(1, Name = "uri")]
        public string DocumentUri { get; set; }

        [ProtoMember(2, Name = "value")]
        public string Value { get; set; }
    }
}
