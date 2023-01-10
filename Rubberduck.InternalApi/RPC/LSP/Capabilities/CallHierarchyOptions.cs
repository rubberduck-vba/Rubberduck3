using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "callHierarchyOptions")]
    public class CallHierarchyOptions : WorkDoneProgressOptions { }
}
