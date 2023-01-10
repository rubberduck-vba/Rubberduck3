using Rubberduck.InternalApi.RPC.LSP;
using Rubberduck.InternalApi.RPC.LSP.Parameters;
using Rubberduck.InternalApi.RPC.LSP.Response;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Rubberduck.Server.Controllers
{
    [ServiceContract]
    public class WorkspaceController
    {
        [OperationContract(Name = "workspace/diagnostic")]
        public async Task<WorkspaceFullDocumentDiagnosticReport> Diagnostic(WorkspaceDiagnosticsParams parameters)
        {
            return null;
        }

        [OperationContract(Name = "workspace/symbol")]
        public async Task<WorkspaceSymbol[]> Symbol(WorkspaceSymbolParams parameters)
        {
            return null;
        }

        [OperationContract(Name = "workspace/configuration")]
        public async Task<LSPAny[]> Configuration(ConfigurationParams parameters)
        {
            return null;
        }

        /// <summary>
        /// Signals a client-side change of configuration settings.
        /// </summary>
        [OperationContract(Name = "workspace/didChangeConfiguration")]
        public async Task DidChangeConfiguration(DidChangeConfigurationParams parameters)
        {
        }

        /// <summary>
        /// Fetches the current list of open workspace folders.
        /// </summary>
        /// <returns>
        /// <c>null</c> if only a single file is open, or an empty array if a workspace is open but no folders are configured.
        /// </returns>
        [OperationContract(Name = "workspace/workspaceFolders")]
        public async Task<WorkspaceFolder[]> WorkspaceFolders()
        {
            return null;
        }

        /// <summary>
        /// Informs the server of a change in workspace folder configurations.
        /// </summary>
        [OperationContract(Name = "workspace/didChangeWorkspaceFolders")]
        public async Task DidChangeWorkspaceFolders(DidChangeWorkspaceFoldersParams parameters)
        {
        }

        /// <summary>
        /// Initiates the creation of files/folders.
        /// </summary>
        [OperationContract(Name = "workspace/willCreateFiles")]
        public async Task<WorkspaceEdit> WillCreateFiles(CreateFilesParams parameters)
        {
            return null;
        }

        /// <summary>
        /// Signals the client-side creation of files/folders.
        /// </summary>
        [OperationContract(Name = "workspace/didCreateFiles")]
        public async Task<WorkspaceEdit> DidCreateFiles(CreateFilesParams parameters)
        {
            return null;
        }

        /// <summary>
        /// Initiates the rename of files/folders.
        /// </summary>
        [OperationContract(Name = "workspace/willRenameFiles")]
        public async Task<WorkspaceEdit> WillRenameFiles(RenameFilesParams parameters)
        {
            return null;
        }

        /// <summary>
        /// Signals the client-side renaming of files/folders.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [OperationContract(Name = "workspace/didRenameFiles")]
        public async Task<WorkspaceEdit> DidRenameFiles(RenameFilesParams parameters)
        {
            return null;
        }

        /// <summary>
        /// Initiates the deletion of files/folders.
        /// </summary>
        [OperationContract(Name = "workspace/willDeleteFiles")]
        public async Task<WorkspaceEdit> WillDeleteFiles(DeleteFilesParams parameters)
        {
            return null;
        }

        /// <summary>
        /// Signals the client-side deletion of files/folders.
        /// </summary>
        [OperationContract(Name = "workspace/didDeleteFiles")]
        public async Task<WorkspaceEdit> DidDeleteFiles(DeleteFilesParams parameters)
        {
            return null;
        }

        /// <summary>
        /// Signals a client-side FileSystemWatcher event.
        /// </summary>
        [OperationContract(Name = "workspace/didChangeWatchedFiles")]
        public async Task DidChangeWatchedFiles(DidChangeWatchedFilesParams parameters)
        {
        }

        /// <summary>
        /// Executes a command on the server.
        /// </summary>
        [OperationContract(Name = "workspace/executeCommand")]
        public async Task ExecuteCommand(ExecuteCommandParams parameters)
        {
        }
    }
}
