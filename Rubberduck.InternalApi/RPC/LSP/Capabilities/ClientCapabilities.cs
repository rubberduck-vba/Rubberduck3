using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    public class ClientCapabilities
    {
        /// <summary>
        /// Workspace-specific client capabilities.
        /// </summary>
        [JsonPropertyName("workspace")]
        public WorkspaceClientCapabilities Workspace { get; set; }

        /// <summary>
        /// TextDocument-specific client capabilities.
        /// </summary>
        [JsonPropertyName("textDocument")]
        public TextDocumentClientCapabilities TextDocument { get; set; }

        // NotebookDocumentClientCapabilities

        /// <summary>
        /// Window-specific client capabilities.
        /// </summary>
        [JsonPropertyName("window")]
        public WindowClientCapabilities Window { get; set; }

        /// <summary>
        /// General client capabilities.
        /// </summary>
        [JsonPropertyName("general")]
        public GeneralClientCapabilities General { get; set; }

        /// <summary>
        /// Experimental client capabilities.
        /// </summary>
        [JsonPropertyName("experimental")]
        public LSPAny Experimental { get; set; }
    }
}
