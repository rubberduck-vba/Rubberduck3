using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "foldingRangeOptions")]
    public class FoldingRangeOptions : WorkDoneProgressOptions { }
}
