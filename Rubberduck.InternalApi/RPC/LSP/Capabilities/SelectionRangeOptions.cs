using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "selectionRangeOptions")]
    public class SelectionRangeOptions : WorkDoneProgressOptions { }
}
