using AustinHarris.JsonRpc;
using Rubberduck.InternalApi.RPC;
using Rubberduck.InternalApi.RPC.LSP;
using Rubberduck.InternalApi.RPC.LSP.Parameters;
using Rubberduck.InternalApi.RPC.LSP.Response;
using Rubberduck.RPC.Platform;
using System.Threading.Tasks;
using WebSocketSharp;

namespace Rubberduck.Server.Controllers
{
    public class WorkspaceController : JsonRpcClient
    {
        public WorkspaceController(WebSocket socket) : base(socket)
        {
        }

        [JsonRpcMethod(JsonRpcMethods.WorkspaceDiagnostics)]
        public async Task<WorkspaceFullDocumentDiagnosticReport> Diagnostic(WorkspaceDiagnosticsParams parameters)
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.WorkspaceDiagnostics, parameters);
                var response = Request<WorkspaceFullDocumentDiagnosticReport>(request);

                return response;
            });
        }

        [JsonRpcMethod(JsonRpcMethods.WorkspaceSymbols)]
        public async Task<WorkspaceSymbol[]> Symbol(WorkspaceSymbolParams parameters)
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.WorkspaceSymbols, parameters);
                var response = Request<WorkspaceSymbol[]>(request);

                return response;
            });
        }

        [JsonRpcMethod(JsonRpcMethods.WorkspaceConfiguration)]
        public async Task<LSPAny[]> Configuration(ConfigurationParams parameters)
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.WorkspaceConfiguration, parameters);
                var response = Request<LSPAny[]>(request);

                return response;
            });
        }

        /// <summary>
        /// Signals a client-side change of configuration settings.
        /// </summary>
        [JsonRpcMethod(JsonRpcMethods.WorkspaceConfigurationDidChange)]
        public async Task DidChangeConfiguration(DidChangeConfigurationParams parameters)
        {
            await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.WorkspaceConfigurationDidChange, parameters);
                Notify(request);
            });
        }

        /// <summary>
        /// Fetches the current list of open workspace folders.
        /// </summary>
        /// <returns>
        /// <c>null</c> if only a single file is open, or an empty array if a workspace is open but no folders are configured.
        /// </returns>
        [JsonRpcMethod(JsonRpcMethods.WorkspaceFolders)]
        public async Task<WorkspaceFolder[]> WorkspaceFolders()
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.WorkspaceFolders, null);
                var response = Request<WorkspaceFolder[]>(request);

                return response;
            });
        }

        /// <summary>
        /// Informs the server of a change in workspace folder configurations.
        /// </summary>
        [JsonRpcMethod(JsonRpcMethods.WorkspaceFoldersDidChange)]
        public async Task DidChangeWorkspaceFolders(DidChangeWorkspaceFoldersParams parameters)
        {
            await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.WorkspaceFoldersDidChange, parameters);
                Notify(request);
            });
        }

        /// <summary>
        /// Initiates the creation of files/folders.
        /// </summary>
        [JsonRpcMethod(JsonRpcMethods.WorkspaceWillCreate)]
        public async Task<WorkspaceEdit> WillCreateFiles(CreateFilesParams parameters)
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.WorkspaceWillCreate, parameters);
                var response = Request<WorkspaceEdit>(request);

                return response;
            });
        }

        /// <summary>
        /// Signals the client-side creation of files/folders.
        /// </summary>
        [JsonRpcMethod(JsonRpcMethods.WorkspaceDidCreate)]
        public async Task<WorkspaceEdit> DidCreateFiles(CreateFilesParams parameters)
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.WorkspaceDidCreate, parameters);
                var response = Request<WorkspaceEdit>(request);

                return response;
            });
        }

        /// <summary>
        /// Initiates the rename of files/folders.
        /// </summary>
        [JsonRpcMethod(JsonRpcMethods.WorkspaceWillRename)]
        public async Task<WorkspaceEdit> WillRenameFiles(RenameFilesParams parameters)
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.WorkspaceWillRename, parameters);
                var response = Request<WorkspaceEdit>(request);

                return response;
            });
        }

        /// <summary>
        /// Signals the client-side renaming of files/folders.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [JsonRpcMethod(JsonRpcMethods.WorkspaceDidRename)]
        public async Task<WorkspaceEdit> DidRenameFiles(RenameFilesParams parameters)
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.WorkspaceDidRename, parameters);
                var response = Request<WorkspaceEdit>(request);

                return response;
            });
        }

        /// <summary>
        /// Initiates the deletion of files/folders.
        /// </summary>
        [JsonRpcMethod(JsonRpcMethods.WorkspaceWillDelete)]
        public async Task<WorkspaceEdit> WillDeleteFiles(DeleteFilesParams parameters)
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.WorkspaceWillDelete, parameters);
                var response = Request<WorkspaceEdit>(request);

                return response;
            });
        }

        /// <summary>
        /// Signals the client-side deletion of files/folders.
        /// </summary>
        [JsonRpcMethod(JsonRpcMethods.WorkspaceDidDelete)]
        public async Task<WorkspaceEdit> DidDeleteFiles(DeleteFilesParams parameters)
        {
            return await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.WorkspaceDidDelete, parameters);
                var response = Request<WorkspaceEdit>(request);

                return response;
            });
        }

        /// <summary>
        /// Signals a client-side FileSystemWatcher event.
        /// </summary>
        [JsonRpcMethod(JsonRpcMethods.WorkspaceWatcherChanged)]
        public async Task DidChangeWatchedFiles(DidChangeWatchedFilesParams parameters)
        {
            await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.WorkspaceWatcherChanged, parameters);
                Notify(request);
            });
        }

        /// <summary>
        /// Executes a command on the server.
        /// </summary>
        [JsonRpcMethod(JsonRpcMethods.WorkspaceExecuteCommand)]
        public async Task ExecuteCommand(ExecuteCommandParams parameters)
        {
            await Task.Run(() =>
            {
                var request = CreateRequest(JsonRpcMethods.WorkspaceExecuteCommand, parameters);
                Notify(request);
            });
        }
    }
}
