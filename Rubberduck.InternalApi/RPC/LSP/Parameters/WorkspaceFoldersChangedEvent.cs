using Rubberduck.InternalApi.RPC.LSP.Response;
using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    public class WorkspaceFoldersChangedEvent
    {
        /// <summary>
        /// An array containing the workspace folders that were added.
        /// </summary>
        [JsonPropertyName("added")]
        public WorkspaceFolder[] Added { get; set; }
        /// <summary>
        /// An array containing the workspace folders that were removed.
        /// </summary>
        [JsonPropertyName("removed")]
        public WorkspaceFolder[] Removed { get; set; }
    }
}
