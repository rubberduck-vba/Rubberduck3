namespace Rubberduck.InternalApi.RPC.LSP
{
    /// <summary>
    /// Represents a string content value that must be interpreted based on its <c>Kind</c>.
    /// </summary>
    public class MarkupContent
    {
        /// <summary>
        /// The type of content.
        /// </summary>
        /// <remarks>
        /// See <c>MarkupKind</c> constants.
        /// </remarks>
        public string Kind { get; set; }

        /// <summary>
        /// The content itself.
        /// </summary>
        public string Value { get; set; }
    }
}
