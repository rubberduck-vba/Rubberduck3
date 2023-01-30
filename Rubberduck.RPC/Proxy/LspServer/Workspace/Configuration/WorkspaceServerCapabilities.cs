using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Workspace.Configuration
{
    public class WorkspaceServerCapabilities
    {
        [JsonPropertyName("workspaceFolders")]
        public WorkspaceFoldersServerCapabilities WorkspaceFolders { get; set; }

        [JsonPropertyName("fileOperations")]
        public FileOperationServerCapabilities FileOperations { get; set; }
    }
}
