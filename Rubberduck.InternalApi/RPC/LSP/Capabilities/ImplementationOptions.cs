using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "implementationOptions")]
    public class ImplementationOptions : WorkDoneProgressOptions { }
}
