using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    public class WorkspaceServerCapabilities
    {
        [JsonPropertyName("workspaceFolders")]
        public WorkspaceFoldersServerCapabilities WorkspaceFolders { get; set; }

        [JsonPropertyName("fileOperations")]
        public FileOperationServerCapabilities FileOperations { get; set; }
    }
}
