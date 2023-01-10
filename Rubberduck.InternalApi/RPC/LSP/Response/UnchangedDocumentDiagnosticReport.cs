using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Response
{
    /// <summary>
    /// A diagnostic report indicating that the previous report is still accurate.
    /// </summary>
    public class UnchangedDocumentDiagnosticReport : FullDocumentDiagnosticReport
    {
        [JsonPropertyName("kind")]
        public override string Kind { get; set; } = Constants.DocumentDiagnosticReportKind.Unchanged;
    }
}
