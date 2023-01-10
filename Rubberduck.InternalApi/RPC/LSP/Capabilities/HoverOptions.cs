using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "hoverOptions")]
    public class HoverOptions : WorkDoneProgressOptions { }
}
