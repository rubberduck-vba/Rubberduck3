using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.LspServer.Workspace.Configuration.Client;
using Rubberduck.RPC.Proxy.LspServer.Window.Configuration.Client;
using Rubberduck.RPC.Proxy.LspServer.TextDocument.File.Configuration.Client;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Configuration.Capabilities
{
    public class ClientCapabilities
    {
        /// <summary>
        /// Workspace-specific client capabilities.
        /// </summary>
        [JsonPropertyName("workspace"), LspCompliant]
        public WorkspaceClientCapabilities Workspace { get; set; }

        /// <summary>
        /// TextDocument-specific client capabilities.
        /// </summary>
        [JsonPropertyName("textDocument"), LspCompliant]
        public TextDocumentClientCapabilities TextDocument { get; set; }

        // NotebookDocumentClientCapabilities

        /// <summary>
        /// Window-specific client capabilities.
        /// </summary>
        [JsonPropertyName("window"), LspCompliant]
        public WindowClientCapabilities Window { get; set; }

        /// <summary>
        /// General client capabilities.
        /// </summary>
        [JsonPropertyName("general"), LspCompliant]
        public GeneralClientCapabilities General { get; set; }

        /// <summary>
        /// Experimental client capabilities.
        /// </summary>
        [JsonPropertyName("experimental"), LspCompliant]
        public object Experimental { get; set; }
    }
}
