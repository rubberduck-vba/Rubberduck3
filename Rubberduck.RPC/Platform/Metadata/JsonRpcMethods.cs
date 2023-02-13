namespace Rubberduck.RPC.Platform.Metadata
{
    /// <summary>
    /// All JsonRpc endpoint paths/names.
    /// </summary>
    /// <remarks>
    /// A <em>request</em> is a <em>method</em> for which the client/caller expects to receive a response.
    /// A <em>notification</em> is an <c>event</c> exposed by the proxy interface.
    /// </remarks>
    public static class JsonRpcMethods
    {
        /// <summary>
        /// All client-to-server endpoints, implemented on the client side by proxy interface members.
        /// </summary>
        /// <remarks>
        /// LSP-compliant endpoints should be marked with a <c>LspCompliantAttribute</c>.
        /// Client-side proxies are generated automatically by StreamJsonRpc.
        /// </remarks>
        public static class ServerProxyRequests
        {
            /// <summary>
            /// Client-to-server request endpoints that share the same definition and model between LSP and LocalDb servers.
            /// </summary>
            public static class Shared
            {
                /// <summary>
                /// Client-to-server shared request endpoints for server-level services.
                /// </summary>
                public static class Server
                {
                    /// <summary>
                    /// An <c>Info</c> request is sent to get detailed server information as a response.
                    /// </summary>
                    [RubberduckSP]public const string Info = "$/info";

                    /// <summary>
                    /// An <c>Initialize</c> request is sent as the first request from a client to the server.
                    /// If the server receives a request or notification before the <c>Initialize</c> request, it should:
                    /// <list type="bullet">
                    /// <item>For a <em>request</em>, respond with error code <c>-32002</c>.</item>
                    /// <item>For a <em>notification</em> that isn't the <c>Exit</c> notification, the request should be dropped.</item>
                    /// </list>
                    /// </summary>
                    /// <remarks>
                    /// Implementation should allow server to <c>Exit</c> without receiving an <c>Initialize</c> request.
                    /// Server may send <c>window/showMessage</c>, <c>window/showMessageRequest</c>, <c>window/logMessage</c>, 
                    /// and <c>telemetry/event</c> requests to the client while the <c>Initialize</c> request is processing.
                    /// </remarks>
                    [LspCompliant] public const string Initialize = "initialize";

                    /// <summary>
                    /// A <c>Initialized</c> notification is sent <em>from the client to the server</em> after the client received an <c>InitializeResult</c>,
                    /// but before the client is sending any other requests or notification to the server.
                    /// </summary>
                    /// <remarks>
                    /// Per LSP an <c>Initialized</c> notification may only be sent once (assumed: <em>per client</em>).
                    /// </remarks>
                    [LspCompliant] public const string Initialized = "initialized";

                    /// <summary>
                    /// Sends a shutdown signal, requesting the termination of the server process.
                    /// </summary>
                    [LspCompliant] public const string Shutdown = "shutdown";

                    /// <summary>
                    /// A notification sent from the client to ask the server to terminate/exit its host process.
                    /// </summary>
                    /// <remarks>
                    /// The server should exit with code 0 (success) if the shutdown request has been received before; otherwise the server process should exit with code 1 (error).
                    /// </remarks>
                    [LspCompliant] public const string Exit = "exit";
                }

                /// <summary>
                /// Client-to-server shared request endpoints for console services.
                /// </summary>
                public static class Console
                {
                    /// <summary>
                    /// A <c>StopTrace</c> notification is sent from a client to the server to stop or pause trace output without changing the server's <c>Trace</c> setting.
                    /// </summary>
                    /// <remarks>
                    /// If <c>Trace</c> level is <c>Off</c> before this notification and later changed to <c>Message</c>, trace would not be output until a <c>$/resumeTrace</c> notification is sent from a client.
                    /// </remarks>
                    [RubberduckSP] public const string StopTrace = "$/stopTrace";
                    
                    /// <summary>
                    /// A <c>ResumeTrace</c> notification is sent from a client to the server to start or resume trace output without changing the server's <c>Trace</c> setting.
                    /// </summary>
                    /// <remarks>
                    /// If <c>Trace</c> level is <c>Off</c> before this notification is sent, trace would not be output until a <c>$/setTrace</c> request is sent to change it to <c>Message</c> or <c>Verbose</c>.
                    /// </remarks>
                    [RubberduckSP] public const string ResumeTrace = "$/resumeTrace";

                    /// <summary>
                    /// A notification that should be used by the client to modify the trace setting of the server.
                    /// </summary>
                    [LspCompliant] public const string SetTrace = "$/setTrace";

                    /// <summary>
                    /// A notification to log the trace of the server’s execution.
                    /// </summary>
                    /// <remarks>
                    /// The amount and content of these notifications depends on the current <c>Trace</c> configuration.
                    /// </remarks>
                    [LspCompliant] public const string LogTrace = "$/logTrace";
                }
            }

            /// <summary>
            /// Rubberduck.Server.LSP-specific client-to-server request endpoints.
            /// </summary>
            public static class LSP
            {
                /// <summary>
                /// LSP client-to-server endpoints for document-level requests.
                /// </summary>
                public static class TextDocument
                {
                    /// <summary>
                    /// File handling endpoints for document-level LSP client-to-server requests.
                    /// </summary>
                    public static class File
                    {
                        [LspCompliant] public const string DidOpen = "textDocument/didOpen";
                        [LspCompliant] public const string DidClose = "textDocument/didClose";
                        [LspCompliant] public const string DidChange = "textDocument/didChange";
                        [LspCompliant] public const string WillSave = "textDocument/willSave";
                        [LspCompliant] public const string WillSaveUntil = "textDocument/willSaveWaitUntil";
                        [LspCompliant] public const string DidSave = "textDocument/didSave";
                    }

                    /// <summary>
                    /// Nagivation tooling endpoints for document-level LSP client-to-server requests.
                    /// </summary>
                    public static class Navigation
                    {
                        [LspCompliant] public const string GoToDeclarations = "textDocument/declaration";
                        [LspCompliant] public const string GoToDefinition = "textDocument/definition";
                        [LspCompliant] public const string GoToTypeDefinition = "textDocument/typeDefinition";
                        [LspCompliant] public const string GoToImplementation = "textDocument/implementation";
                        [LspCompliant] public const string FindReferences = "textDocument/references";

                        // codelens
                        [LspCompliant] public const string CodeLens = "textDocument/codeLens";
                        [LspCompliant] public const string ResolveCodeLens = "codeLens/resolve";

                        // links
                        [LspCompliant] public const string DocumentLinks = "textDocument/documentLink";
                        [LspCompliant] public const string ResolveDocumentLink = "documentLink/resolve";
                    }

                    /// <summary>
                    /// Language query endpoints for document-level LSP client-to-server requests.
                    /// </summary>
                    public static class Language
                    {
                        // new!
                        [LspCompliant] public const string PrepareCallHierarchy = "textDocument/prepareCallHierarchy";
                        [LspCompliant] public const string CallHierarchyIncoming = "callHierarchy/incomingCalls";
                        [LspCompliant] public const string CallHierarchyOutgoing = "callHierarchy/outgoingCalls";

                        // "implements" and document module subtypes
                        [LspCompliant] public const string PrepareTypeHierarchy = "textDocument/prepareTypeHierarchy";
                        [LspCompliant] public const string Supertypes = "typeHierarchy/supertypes";
                        [LspCompliant] public const string Subtypes = "typeHierarchy/subtypes";

                        // refactor-rename
                        [LspCompliant] public const string Rename = "textDocument/rename";
                        [LspCompliant] public const string PrepareRename = "textDocument/prepareRename";

                        // "inspections"
                        [LspCompliant] public const string Diagnostics = "textDocument/diagnostic";

                        // "quickfixes"
                        [LspCompliant] public const string CodeAction = "textDocument/codeAction";
                        [LspCompliant] public const string ResolveCodeAction = "codeAction/resolve";

                        // symbols
                        [LspCompliant] public const string Monikers = "textDocument/moniker";
                        [LspCompliant] public const string Symbols = "textDocument/documentSymbol";
                        [LspCompliant] public const string SemanticTokens = "textDocument/semanticTokens";
                        [LspCompliant] public const string SemanticTokensFull = "textDocument/semanticTokens/full";
                        [LspCompliant] public const string SemanticTokensDelta = "textDocument/semanticTokens/full/delta";
                    }

                    /// <summary>
                    /// Editor helper endpoints for document-level LSP client-to-server requests.
                    /// </summary>
                    public static class Editor
                    {
                        [LspCompliant] public const string Hover = "textDocument/hover";

                        // inline completion dropdowns
                        [LspCompliant] public const string Completion = "textDocument/completion";
                        [LspCompliant] public const string ResolveCompletionItem = "completionItem/resolve";

                        // intellisense
                        [LspCompliant] public const string SignatureHelp = "textDocument/signatureHelp";

                        [LspCompliant] public const string FoldingRanges = "textDocument/foldingRange";

                        [LspCompliant] public const string Highlights = "textDocument/documentHighlight";

                        // indentation, bloc completion
                        [LspCompliant] public const string Formatting = "textDocument/formatting";
                        [LspCompliant] public const string RangeFormatting = "textDocument/rangeFormatting";
                        [LspCompliant] public const string OnTypeFormatting = "textDocument/onTypeFormatting";

                        // in-editor color info
                        [LspCompliant] public const string Color = "textDocument/documentColor";
                        [LspCompliant] public const string ColorPresentation = "textDocument/colorPresentation";

                        [LspCompliant] public const string SelectionRange = "textDocument/selectionRange";
                        [LspCompliant] public const string LinkedEditingRanges = "textDocument/linkedEditingRanges";

                        [LspCompliant] public const string InlayHints = "textDocument/inlayHint";
                        [LspCompliant] public const string ResolveInlayHint = "inlayHint/resolve";
                    }
                }

                /// <summary>
                /// LSP client-to-server endpoints for workspace-level requests.
                /// </summary>
                public static class Workspace
                {
                    /// <summary>
                    /// Server endpoints for workspace-level LSP client-to-server requests.
                    /// </summary>
                    public static class Server
                    {
                        [LspCompliant] public const string Configuration = "workspace/configuration";
                        [LspCompliant] public const string ConfigurationDidChange = "workspace/didChangeConfiguration";
                        [LspCompliant] public const string ExecuteCommand = "workspace/executeCommand";
                    }

                    /// <summary>
                    /// File handling endpoints for workspace-level LSP client-to-server requests.
                    /// </summary>
                    public static class File
                    {
                        [LspCompliant] public const string WatcherChanged = "workspace/didChangeWatchedFiles";

                        [LspCompliant] public const string Folders = "workspace/workspaceFolders";
                        [LspCompliant] public const string FoldersDidChange = "workspace/didChangeWorkspaceFolders";

                        [LspCompliant] public const string WillCreate = "workspace/willCreateFiles";
                        [LspCompliant] public const string DidCreate = "workspace/didCreateFiles";

                        [LspCompliant] public const string WillRename = "workspace/willRenameFiles";
                        [LspCompliant] public const string DidRename = "workspace/didRenameFiles";

                        [LspCompliant] public const string WillDelete = "workspace/willDeleteFiles";
                        [LspCompliant] public const string DidDelete = "workspace/didDeleteFiles";
                    }

                    /// <summary>
                    /// Language query endpoints for workspace-level LSP client-to-server requests.
                    /// </summary>
                    public static class Language
                    {
                        [LspCompliant] public const string Diagnostics = "workspace/diagnostic";
                        [LspCompliant] public const string Symbols = "workspace/symbol";
                        [LspCompliant] public const string ResolveSymbol = "workspaceSymbol/resolve";
                    }
                }
            }

            /// <summary>
            /// Rubberduck.Server.LocalDb-specific client-to-server request endpoints
            /// </summary>
            public static class LocalDb
            {
                /* TODO */

            }
        }

        /// <summary>
        /// All server-to-client endpoints, implemented on the server side by proxy interface members.
        /// </summary>
        /// <remarks>
        /// LSP-compliant endpoints should be marked with a <c>LspCompliantAttribute</c>.
        /// Server-side proxies are generated automatically by StreamJsonRpc.
        /// </remarks>
        public static class ClientProxyRequests
        {
            public static class Telemetry
            {
                [LspCompliant] public const string TelemetryEvent = "telemetry/event";

            }

            public static class LSP
            {
                [LspCompliant] public const string LogMessage = "window/logMessage";

                [LspCompliant] public const string ShowMessage = "window/showMessage";
                [LspCompliant] public const string ShowMessageRequest = "window/showMessageRequest";

                [LspCompliant] public const string ShowDocument = "window/showDocument";

                [LspCompliant] public const string Progress = "$/progress";
                [LspCompliant] public const string CreateWorkDoneProgress = "window/workDoneProgress/create";
                [LspCompliant] public const string CancelWorkDoneProgress = "window/workDoneProgress/cancel";

                [LspCompliant] public const string PublishDocumentDiagnostics = "textDocument/publishDiagnostics";

                [LspCompliant] public const string RefreshCodeLens = "workspace/codeLens/refresh";
                [LspCompliant] public const string RefreshSemanticTokens = "workspace/semanticTokens/refresh";
                [LspCompliant] public const string RefreshInlayHints = "workspace/inlayHint/refresh";
                [LspCompliant] public const string RefreshDiagnostics = "workspace/diagnostic/refresh";

                [LspCompliant] public const string ApplyWorkspaceEdit = "workspace/applyEdit";
            }

            public static class LocalDb
            {
                /* TODO */
            }
        }
    }
}
