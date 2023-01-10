using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "documentColorOptions")]
    public class DocumentColorOptions : WorkDoneProgressOptions { }
}
