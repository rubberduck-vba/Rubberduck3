using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "linkedEditingRangeOptions")]
    public class LinkedEditingRangeOptions : WorkDoneProgressOptions { }
}
