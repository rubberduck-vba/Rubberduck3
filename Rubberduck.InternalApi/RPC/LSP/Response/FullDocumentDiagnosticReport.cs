using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Response
{
    /// <summary>
    /// A diagnostic report with a full set of diagnostics.
    /// </summary>
    public class FullDocumentDiagnosticReport
    {
        [JsonPropertyName("kind")]
        public virtual string Kind { get; set; } = Constants.DocumentDiagnosticReportKind.Full;

        /// <summary>
        /// An optional result ID. If specified, will be sent with the next diagnostic report for the same document.
        /// </summary>
        [JsonPropertyName("resultId")]
        public string ResultId { get; set; }

        /// <summary>
        /// The diagnostic items.
        /// </summary>
        [JsonPropertyName("items")]
        public Diagnostic[] Items { get; set; }
    }
}
