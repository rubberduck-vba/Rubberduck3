using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    /// <summary>
    /// An event describing a change to a text document.
    /// </summary>
    [ProtoContract(Name = "textDocumentContentChangeEvent")]
    public class TextDocumentContentChangeEvent
    {
        /// <summary>
        /// The range of the document that changed.
        /// </summary>
        [ProtoMember(1, Name = "range")]
        public Range Range { get; set; }

        /// <summary>
        /// The new text for the provided range.
        /// </summary>
        /// <remarks>
        /// If <c>Range</c> is <c>null</c>, contains the text for the full document.
        /// </remarks>
        [ProtoMember(2, Name = "text")]
        public string Text { get; set; }
    }
}
