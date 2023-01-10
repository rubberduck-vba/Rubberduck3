using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "monikerOptions")]
    public class MonikerOptions : WorkDoneProgressOptions { }
}
