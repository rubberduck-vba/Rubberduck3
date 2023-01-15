using System.Text.Json.Serialization;
namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    public class DidChangeTextDocumentParams
    {
        /// <summary>
        /// The document that was changed. The version number points to the version after all provided content changes have been applied.
        /// </summary>
        [JsonPropertyName("textDocument")]
        public VersionedTextDocumentIdentifier TextDocument { get; set; }

        [JsonPropertyName("contentChanges")]
        public TextDocumentContentChangeEvent[] ContentChanges { get; set; }
    }
}
