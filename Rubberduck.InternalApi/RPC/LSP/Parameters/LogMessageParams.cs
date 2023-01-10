using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract(Name = "logMessageParams")]
    public class LogMessageParams
    {
        [ProtoMember(1, Name = "messageType")]
        public int MessageType { get; set; } = Constants.MessageType.Log;

        [ProtoMember(2, Name = "message")]
        public string Message { get; set; }
    }
}
