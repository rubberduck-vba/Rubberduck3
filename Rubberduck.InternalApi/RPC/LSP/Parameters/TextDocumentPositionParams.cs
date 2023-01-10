using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract]
    public interface ITextDocumentPositionParams
    {
        /// <summary>
        /// The text document.
        /// </summary>
        [ProtoMember(1, Name = "textDocument")]
        TextDocumentIdentifier TextDocument { get; set; }

        /// <summary>
        /// The position inside the text document.
        /// </summary>
        [ProtoMember(2, Name = "position")]
        Position Position { get; set; }
    }

    /// <summary>
    /// A parameter literal used in requests to pass a text document and a position inside that document.
    /// </summary>
    [ProtoContract]
    public class TextDocumentPositionParams : ITextDocumentPositionParams
    {
        /// <summary>
        /// The text document.
        /// </summary>
        [ProtoMember(1, Name = "textDocument")]
        public TextDocumentIdentifier TextDocument { get; set; }

        /// <summary>
        /// The position inside the text document.
        /// </summary>
        [ProtoMember(2, Name = "position")]
        public Position Position { get; set; }
    }
}
