using Rubberduck.RPC.Proxy.LspServer.Workspace.Language.Configuration.Client;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Workspace.Configuration.Client
{
    public class WorkspaceClientCapabilities
    {
        /// <summary>
        /// <c>true</c> if client supports applying batch edits to the workspace with 'workspace/applyEdit'.
        /// </summary>
        [JsonPropertyName("applyEdit")]
        public bool? ApplyEdit { get; set; }

        /// <summary>
        /// Capabilities specific to workspace edits.
        /// </summary>
        [JsonPropertyName("workspaceEdit")]
        public WorkspaceEditClientCapabilities WorkspaceEdit { get; set; }

        /// <summary>
        /// Capabilities specific to the 'workspace/didChangeConfiguration' notification.
        /// </summary>
        [JsonPropertyName("didChangeConfiguration")]
        public DidChangeConfigurationClientCapabilities DidChangeConfiguration { get; set; }

        /// <summary>
        /// Capabilities specific to the 'workspace/didChangeWatchedFiles' notification.
        /// </summary>
        [JsonPropertyName("didChangeWatchedFiles")]
        public DidChangeWatchedFilesClientCapabilities DidChangeWatchedFiles { get; set; }

        /// <summary>
        /// Capabilities specific to the 'workspace/symbol' request.
        /// </summary>
        [JsonPropertyName("symbol")]
        public WorkspaceSymbolClientCapabilities Symbol { get; set; }

        /// <summary>
        /// Capabilities specific to the 'workspace/executeCommand' request.
        /// </summary>
        [JsonPropertyName("executeCommand")]
        public ExecuteCommandClientCapabilities ExecuteCommand { get; set; }

        /// <summary>
        /// Whether the client supports workspace folders.
        /// </summary>
        [JsonPropertyName("workspaceFolders")]
        public bool WorkspaceFolders { get; set; }

        /// <summary>
        /// Whether the client supports 'workspace/configuration' requests.
        /// </summary>
        [JsonPropertyName("configuration")]
        public bool Configuration { get; set; }

        /// <summary>
        /// Capabilities specific to semantic token requests scoped to the workspace.
        /// </summary>
        [JsonPropertyName("semanticTokens")]
        public SemanticTokenWorkspaceClientCapabilities SemanticTokens { get; set; }

        /// <summary>
        /// Capabilities specific to code lens requests scoped to the workspace.
        /// </summary>
        [JsonPropertyName("codeLens")]
        public CodeLensWorkspaceClientCapabilities CodeLens { get; set; }

        /// <summary>
        /// Client support for file requests/notifications.
        /// </summary>
        [JsonPropertyName("fileOperations")]
        public FileOperationCapabilities FileOperations { get; set; }

        /// <summary>
        /// Client workspace capabilities specific to inline values.
        /// </summary>
        [JsonPropertyName("inlineValues")]
        public InlineValuesWorkspaceClientCapabilities InlineValues { get; set; }

        /// <summary>
        /// Client workspace capabilities specific to inlay hints.
        /// </summary>
        [JsonPropertyName("inlayHints")]
        public InlayHintWorkspaceClientCapabilities InlayHints { get; set; }

        /// <summary>
        /// Client workspace capabilities specific to diagnostics.
        /// </summary>
        [JsonPropertyName("diagnostics")]
        public DiagnosticWorkspaceClientCapabilities Diagnostics { get; set; }

        public class FileOperationCapabilities
        {
            /// <summary>
            /// Whether client supports dynamic registration for file requests/notifications.
            /// </summary>
            [JsonPropertyName("dynamicRegistration")]
            public bool SupportsDynamicRegistration { get; set; }

            /// <summary>
            /// <c>true</c> if client has support for sending <c>didCreateFiles</c> notifications.
            /// </summary>
            [JsonPropertyName("didCreate")]
            public bool DidCreate { get; set; }

            /// <summary>
            /// <c>true</c> if client has support for sending <c>willCreateFiles</c> notifications.
            /// </summary>
            [JsonPropertyName("willCreate")]
            public bool WillCreate { get; set; }

            /// <summary>
            /// <c>true</c> if client has support for sending <c>didRenameFiles</c> notifications.
            /// </summary>
            [JsonPropertyName("didRename")]
            public bool DidRename { get; set; }

            /// <summary>
            /// <c>true</c> if client has support for sending <c>willRenameFiles</c> notifications.
            /// </summary>
            [JsonPropertyName("willRename")]
            public bool WillRename { get; set; }

            /// <summary>
            /// <c>true</c> if client has support for sending <c>didDeleteFiles</c> notifications.
            /// </summary>
            [JsonPropertyName("didDelete")]
            public bool DidDelete { get; set; }

            /// <summary>
            /// <c>true</c> if client has support for sending <c>willDeleteFiles</c> notifications.
            /// </summary>
            [JsonPropertyName("willDelete")]
            public bool WillDelete { get; set; }
        }
    }
}
