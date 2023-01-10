using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract(Name = "showMessageParams")]
    public class ShowMessageParams
    {
        [ProtoMember(1, Name = "type")]
        public int MessageType { get; set; } = Constants.MessageType.Info;

        [ProtoMember(2, Name = "message")]
        public string Message { get; set; }
    }
}
