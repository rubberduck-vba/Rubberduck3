using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Workspace.File.Model
{
    public class WorkspaceFoldersChangedEvent
    {
        /// <summary>
        /// An array containing the workspace folders that were added.
        /// </summary>
        [JsonPropertyName("added"), LspCompliant]
        public WorkspaceFolder[] Added { get; set; }
        /// <summary>
        /// An array containing the workspace folders that were removed.
        /// </summary>
        [JsonPropertyName("removed"), LspCompliant]
        public WorkspaceFolder[] Removed { get; set; }
    }
}
