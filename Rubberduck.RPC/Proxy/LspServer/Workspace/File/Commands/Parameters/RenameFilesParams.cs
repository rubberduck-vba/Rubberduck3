using Rubberduck.RPC.Proxy.LspServer.Workspace.File.Model;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Workspace.File.Commands.Parameters
{
    public class RenameFilesParams
    {
        /// <summary>
        /// The files/folders renamed in this operation.
        /// </summary>
        [JsonPropertyName("files")]
        public FileRename[] Files { get; set; }
    }
}
