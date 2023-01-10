using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "typeDefinitionOptions")]
    public class TypeDefinitionOptions : WorkDoneProgressOptions { }
}
