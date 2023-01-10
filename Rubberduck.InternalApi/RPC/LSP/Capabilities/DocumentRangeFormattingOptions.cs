using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "documentRangeFormattingOptions")]
    public class DocumentRangeFormattingOptions : WorkDoneProgressOptions { }
}
