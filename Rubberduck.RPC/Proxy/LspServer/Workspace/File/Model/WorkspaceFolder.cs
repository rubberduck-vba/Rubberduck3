using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Workspace.File.Model
{
    public class WorkspaceFolder
    {
        /// <summary>
        /// The associated URI for this workspace folder.
        /// </summary>
        [JsonPropertyName("documentUri"), LspCompliant]
        public string DocumentUri { get; set; }

        /// <summary>
        /// The name of the workspace folder.
        /// </summary>
        [JsonPropertyName("name"), LspCompliant]
        public string Name { get; set; }
    }
}
