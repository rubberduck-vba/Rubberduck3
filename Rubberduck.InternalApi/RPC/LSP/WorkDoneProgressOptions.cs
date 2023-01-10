using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP
{
    [ProtoContract]
    public class WorkDoneProgressOptions
    {
        [ProtoMember(1, Name = "workDoneProgress")]
        public bool WorkDoneProgress { get; set; }
    }
}
