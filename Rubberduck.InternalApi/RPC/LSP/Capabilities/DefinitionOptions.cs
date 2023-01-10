using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "definitionOptions")]
    public class DefinitionOptions : WorkDoneProgressOptions { }
}
