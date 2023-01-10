using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "declarationOptions")]
    public class DeclarationOptions : WorkDoneProgressOptions { }
}
