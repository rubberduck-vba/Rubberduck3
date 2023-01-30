using Rubberduck.RPC.Proxy.LspServer.Workspace.File.Model;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Workspace.File.Commands.Parameters
{
    public class CreateFilesParams
    {
        /// <summary>
        /// An array of all files/folders created in this operation.
        /// </summary>
        [JsonPropertyName("files")]
        public FileCreate[] Files { get; set; }
    }
}
