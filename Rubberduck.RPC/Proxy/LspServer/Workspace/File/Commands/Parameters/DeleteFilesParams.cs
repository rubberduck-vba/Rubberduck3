using Rubberduck.RPC.Proxy.LspServer.Workspace.File.Model;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Workspace.File.Commands.Parameters
{
    public class DeleteFilesParams
    {
        /// <summary>
        /// The files/folders deleted in this operation.
        /// </summary>
        [JsonPropertyName("files")]
        public FileDelete[] Files { get; set; }
    }
}
