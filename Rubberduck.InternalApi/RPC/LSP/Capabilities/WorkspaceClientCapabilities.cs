using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "workspaceClientCapabilities")]
    public class WorkspaceClientCapabilities
    {
        /// <summary>
        /// <c>true</c> if client supports applying batch edits to the workspace with 'workspace/applyEdit'.
        /// </summary>
        [ProtoMember(1, Name = "applyEdit")]
        public bool? ApplyEdit { get; set; }

        /// <summary>
        /// Capabilities specific to workspace edits.
        /// </summary>
        [ProtoMember(2, Name = "workspaceEdit")]
        public WorkspaceEditClientCapabilities WorkspaceEdit { get; set; }

        /// <summary>
        /// Capabilities specific to the 'workspace/didChangeConfiguration' notification.
        /// </summary>
        [ProtoMember(3, Name = "didChangeConfiguration")]
        public DidChangeConfigurationClientCapabilities DidChangeConfiguration { get; set; }

        /// <summary>
        /// Capabilities specific to the 'workspace/didChangeWatchedFiles' notification.
        /// </summary>
        [ProtoMember(4, Name = "didChangeWatchedFiles")]
        public DidChangeWatchedFilesClientCapabilities DidChangeWatchedFiles { get; set; }

        /// <summary>
        /// Capabilities specific to the 'workspace/symbol' request.
        /// </summary>
        [ProtoMember(5, Name = "symbol")]
        public WorkspaceSymbolClientCapabilities Symbol { get; set; }

        /// <summary>
        /// Capabilities specific to the 'workspace/executeCommand' request.
        /// </summary>
        [ProtoMember(6, Name = "executeCommand")]
        public ExecuteCommandClientCapabilities ExecuteCommand { get; set; }

        /// <summary>
        /// Whether the client supports workspace folders.
        /// </summary>
        [ProtoMember(7, Name = "workspaceFolders")]
        public bool WorkspaceFolders { get; set; }

        /// <summary>
        /// Whether the client supports 'workspace/configuration' requests.
        /// </summary>
        [ProtoMember(8, Name = "configuration")]
        public bool Configuration { get; set; }

        /// <summary>
        /// Capabilities specific to semantic token requests scoped to the workspace.
        /// </summary>
        [ProtoMember(9, Name = "semanticTokens")]
        public SemanticTokenWorkspaceClientCapabilities SemanticTokens { get; set; }

        /// <summary>
        /// Capabilities specific to code lens requests scoped to the workspace.
        /// </summary>
        [ProtoMember(10, Name = "codeLens")]
        public CodeLensWorkspaceClientCapabilities CodeLens { get; set; }

        /// <summary>
        /// Client support for file requests/notifications.
        /// </summary>
        [ProtoMember(11, Name = "fileOperations")]
        public FileOperationCapabilities FileOperations { get; set; }

        /// <summary>
        /// Client workspace capabilities specific to inline values.
        /// </summary>
        [ProtoMember(12, Name = "inlineValues")]
        public InlineValuesWorkspaceClientCapabilities InlineValues { get; set; }

        /// <summary>
        /// Client workspace capabilities specific to inlay hints.
        /// </summary>
        [ProtoMember(13, Name = "inlayHints")]
        public InlayHintWorkspaceClientCapabilities InlayHints { get; set; }

        /// <summary>
        /// Client workspace capabilities specific to diagnostics.
        /// </summary>
        [ProtoMember(14, Name = "diagnostics")]
        public DiagnosticWorkspaceClientCapabilities Diagnostics { get; set; }

        [ProtoContract(Name = "fileOperationCapabilities")]
        public class FileOperationCapabilities
        {
            /// <summary>
            /// Whether client supports dynamic registration for file requests/notifications.
            /// </summary>
            [ProtoMember(1, Name = "dynamicRegistration")]
            public bool SupportsDynamicRegistration { get; set; }

            /// <summary>
            /// <c>true</c> if client has support for sending <c>didCreateFiles</c> notifications.
            /// </summary>
            [ProtoMember(2, Name = "didCreate")]
            public bool DidCreate { get; set; }

            /// <summary>
            /// <c>true</c> if client has support for sending <c>willCreateFiles</c> notifications.
            /// </summary>
            [ProtoMember(3, Name = "willCreate")]
            public bool WillCreate { get; set; }

            /// <summary>
            /// <c>true</c> if client has support for sending <c>didRenameFiles</c> notifications.
            /// </summary>
            [ProtoMember(4, Name = "didRename")]
            public bool DidRename { get; set; }

            /// <summary>
            /// <c>true</c> if client has support for sending <c>willRenameFiles</c> notifications.
            /// </summary>
            [ProtoMember(5, Name = "willRename")]
            public bool WillRename { get; set; }

            /// <summary>
            /// <c>true</c> if client has support for sending <c>didDeleteFiles</c> notifications.
            /// </summary>
            [ProtoMember(6, Name = "didDelete")]
            public bool DidDelete { get; set; }

            /// <summary>
            /// <c>true</c> if client has support for sending <c>willDeleteFiles</c> notifications.
            /// </summary>
            [ProtoMember(7, Name = "willDelete")]
            public bool WillDelete { get; set; }
        }
    }
}
