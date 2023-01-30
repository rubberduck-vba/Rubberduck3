using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Workspace.File.Model
{
    public class ApplyWorkspaceEditResult
    {
        /// <summary>
        /// Whether the edit was applied or not.
        /// </summary>
        [JsonPropertyName("applied")]
        public bool Applied { get; set; }

        /// <summary>
        /// An optional reason why the edit was not applied.
        /// </summary>
        [JsonPropertyName("failureReason")]
        public string FailureReason { get; set; }

        /// <summary>
        /// Depending on the client's failure handling strategy, may contain the index of the change that failed.
        /// </summary>
        [JsonPropertyName("failedChange")]
        public int? FailedChange { get; set; }
    }
}
