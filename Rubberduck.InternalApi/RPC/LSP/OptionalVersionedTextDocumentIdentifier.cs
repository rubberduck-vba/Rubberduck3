using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP
{
    /// <summary>
    /// An identifier which optionally denotes a specific version of a text document.
    /// </summary>
    /// <remarks>
    /// This information usually flows from the server to the client.
    /// </remarks>
    public class OptionalVersionedTextDocumentIdentifier : TextDocumentIdentifier
    {
        /// <summary>
        /// The version number of this document (it will increase after each change, including undo/redo).
        /// </summary>
        [JsonPropertyName("version")]
        public int? Version { get; set; }
    }
}
