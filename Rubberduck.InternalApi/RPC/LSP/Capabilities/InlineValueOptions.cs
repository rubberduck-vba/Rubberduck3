using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "inlineValueOptions")]
    public class InlineValueOptions : WorkDoneProgressOptions { }
}
