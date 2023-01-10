using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "typeHierarchyOptions")]
    public class TypeHierarchyOptions : WorkDoneProgressOptions { }
}
