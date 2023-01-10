using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "documentSymbolProvider")]
    public class DocumentSymbolProvider : WorkDoneProgressOptions
    {
        /// <summary>
        /// A human-readable string that is shown when multiple outline trees are shown for the same document.
        /// </summary>
        [ProtoMember(2, Name = "label")]
        public string Label { get; set; }
    }
}
