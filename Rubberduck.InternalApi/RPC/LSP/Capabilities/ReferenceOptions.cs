using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "referenceOptions")]
    public class ReferenceOptions : WorkDoneProgressOptions { }
}
