using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    public class DidChangeWorkspaceFoldersParams
    {
        /// <summary>
        /// Describes the workspace folders configuration changes.
        /// </summary>
        [JsonPropertyName("event")]
        public WorkspaceFoldersChangedEvent Event { get; set; }
    }
}
