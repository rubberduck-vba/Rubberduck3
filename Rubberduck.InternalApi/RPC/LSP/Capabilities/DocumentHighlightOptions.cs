using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "documentHighlightOptions")]
    public class DocumentHighlightOptions : WorkDoneProgressOptions { }
}
