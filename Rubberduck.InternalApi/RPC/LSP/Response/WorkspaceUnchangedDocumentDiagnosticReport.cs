using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Response
{
    public class WorkspaceUnchangedDocumentDiagnosticReport : WorkspaceFullDocumentDiagnosticReport 
    {
        public override string Kind { get; set; } = Constants.DocumentDiagnosticReportKind.Unchanged;
    }

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

    public class ShowDocumentResult
    {
        /// <summary>
        /// Whether the document was successfully shown.
        /// </summary>
        [JsonPropertyName("success")]
        public bool Success { get; set; }
    }
}
