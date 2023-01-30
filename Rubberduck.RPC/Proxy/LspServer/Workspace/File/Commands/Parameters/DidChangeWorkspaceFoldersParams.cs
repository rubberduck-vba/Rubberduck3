using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.LspServer.Workspace.File.Model;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Workspace.File.Commands.Parameters
{
    public class DidChangeWorkspaceFoldersParams
    {
        /// <summary>
        /// Describes the workspace folders configuration changes.
        /// </summary>
        [JsonPropertyName("event"), LspCompliant]
        public WorkspaceFoldersChangedEvent Event { get; set; }
    }
}
