using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.LspServer.TextDocument.Model;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.File.Commands.Parameters
{
    public class DidChangeTextDocumentParams
    {
        /// <summary>
        /// The document that was changed. The version number points to the version after all provided content changes have been applied.
        /// </summary>
        [JsonPropertyName("textDocument"), LspCompliant]
        public VersionedTextDocumentIdentifier TextDocument { get; set; }

        [JsonPropertyName("contentChanges"), LspCompliant]
        public TextDocumentContentChangeEvent[] ContentChanges { get; set; }
    }
}
