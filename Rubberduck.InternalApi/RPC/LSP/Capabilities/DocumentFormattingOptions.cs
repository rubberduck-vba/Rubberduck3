using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "documentFormattingOptions")]
    public class DocumentFormattingOptions : WorkDoneProgressOptions { }
}
