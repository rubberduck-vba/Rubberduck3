using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubberduck.InternalApi.RPC.LSP
{
    public static class JsonRpcMethods
    {
        // client -> server requests and notifications
        public const string Initialize = "initialize";
        public const string Initialized = "initialized";
        public const string Shutdown = "shutdown";
        public const string Exit = "exit";
        public const string SetTrace = "$/setTrace";
        public const string LogTrace = "$/logTrace";

        public const string CallHierarchyIncoming = "callHierarchy/incomingCalls";
        public const string CallHierarchyOutgoing = "callHierarchy/outgoingCalls";
        public const string ResolveCodeAction = "codeAction/resolve";
        public const string ResolveCodeLens = "codeLens/resolve";
        public const string ResolveCompletionItem = "completionItem/resolve";
        public const string ResolveDocumentLink = "documentLink/resolve";
        public const string ResolveInlayHint = "inlayHint/resolve";
        public const string ResolveWorkspaceSymbol = "workspaceSymbol/resolve";
        public const string DidOpen = "textDocument/didOpen";
        public const string DidChange = "textDocument/didChange";
        public const string WillSave = "textDocument/willSave";
        public const string WillSaveUntil = "textDocument/willSaveWaitUntil";
        public const string DidSave = "textDocument/didSave";
        public const string DidClose = "textDocument/didClose";
        public const string GoToDeclarations = "textDocument/declaration";
        public const string GoToDefinition = "textDocument/definition";
        public const string GoToTypeDefinition = "textDocument/typeDefinition";
        public const string GoToImplementation = "textDocument/implementation";
        public const string FindReferences = "textDocument/references";
        public const string PrepareCallHierarchy = "textDocument/prepareCallHierarchy";
        public const string PrepareTypeHierarchy = "textDocument/prepareTypeHierarchy";
        public const string DocumentHighlights = "textDocument/documentHighlight";
        public const string DocumentLinks = "textDocument/documentLink";
        public const string DocumentHover = "textDocument/hover";
        public const string DocumentCodeLens = "textDocument/codeLens";
        public const string DocumentFoldingRanges = "textDocument/foldingRange";
        public const string DocumentSelectionRange = "textDocument/selectionRange";
        public const string DocumentSymbols = "textDocument/documentSymbol";
        public const string DocumentSemanticTokens = "textDocument/semanticTokens";
        public const string DocumentSemanticTokensFull = "textDocument/semanticTokens/full";
        public const string DocumentSemanticTokensDelta = "textDocument/semanticTokens/full/delta";
        public const string DocumentInlayHints = "textDocument/inlayHint";
        public const string DocumentMonikers = "textDocument/moniker";
        public const string DocumentDiagnostics = "textDocument/diagnostic";
        public const string Completion = "textDocument/completion";
        public const string SignatureHelp = "textDocument/signatureHelp";
        public const string CodeAction = "textDocument/codeAction";
        public const string Color = "textDocument/documentColor";
        public const string ColorPresentation = "textDocument/colorPresentation";
        public const string Formatting = "textDocument/formatting";
        public const string RangeFormatting = "textDocument/rangeFormatting";
        public const string OnTypeFormatting = "textDocument/onTypeFormatting";
        public const string Rename = "textDocument/rename";
        public const string PrepareRename = "textDocument/prepareRename";
        public const string LinkedEditingRanges = "textDocument/linkedEditingRanges";
        public const string SuperTypes = "typeHierarchy/supertypes";
        public const string SubTypes = "typeHierarchy/subtypes";
        public const string WorkspaceDiagnostics = "workspace/diagnostic";
        public const string WorkspaceSymbols = "workspace/symbol";
        public const string WorkspaceConfiguration = "workspace/configuration";
        public const string WorkspaceConfigurationDidChange = "workspace/didChangeConfiguration";
        public const string WorkspaceFolders = "workspace/workspaceFolders";
        public const string WorkspaceFoldersDidChange = "workspace/didChangeWorkspaceFolders";
        public const string WorkspaceWillCreate = "workspace/willCreateFiles";
        public const string WorkspaceDidCreate = "workspace/didCreateFiles";
        public const string WorkspaceWillRename = "workspace/willRenameFiles";
        public const string WorkspaceDidRename = "workspace/didRenameFiles";
        public const string WorkspaceWillDelete = "workspace/willDeleteFiles";
        public const string WorkspaceDidDelete = "workspace/didDeleteFiles";
        public const string WorkspaceWatcherChanged = "workspace/didChangeWatchedFiles";
        public const string WorkspaceExecuteCommand = "workspace/executeCommand";

        // server -> client requests and notifications
        public const string TelemetryEvent = "telemetry/event";
        public const string PublishTextDocumentDiagnostics = "textDocument/publishDiagnostics";
        public const string ShowMessage = "window/showMessage";
        public const string ShowMessageRequest = "window/showMessageRequest";
        public const string ShowDocument = "window/showDocument";
        public const string LogMessage = "window/logMessage";
        public const string CreateWorkDoneProgress = "window/workDoneProgress/create";
        public const string CancelWorkDoneProgress = "window/workDoneProgress/cancel";
        public const string RefreshCodeLens = "workspace/codeLens/refresh";
        public const string RefreshSemanticTokens = "workspace/semanticTokens/refresh";
        public const string RefreshInlayHints = "workspace/inlayHint/refresh";
        public const string RefreshDiagnostics = "workspace/diagnostic/refresh";
        public const string ApplyWorkspaceEdit = "workspace/applyEdit";
    }
}
