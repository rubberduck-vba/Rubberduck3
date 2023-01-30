using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Model
{
    /// <summary>
    /// An identifier to denote a specific version of a text document.
    /// </summary
    /// <remarks>
    /// This information usually flows from the client to the server.
    /// </remarks>
    public class VersionedTextDocumentIdentifier : TextDocumentIdentifier
    {
        /// <summary>
        /// The version number of this document (it will increase after each change, including undo/redo).
        /// </summary>
        [JsonPropertyName("version"), LspCompliant]
        public int Version { get; set; }
    }
}
