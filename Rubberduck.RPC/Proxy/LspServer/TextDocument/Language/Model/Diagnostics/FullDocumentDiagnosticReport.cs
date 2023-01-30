using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Model
{
    /// <summary>
    /// A diagnostic report with a full set of diagnostics.
    /// </summary>
    public class FullDocumentDiagnosticReport
    {
        [JsonPropertyName("kind"), JsonConverter(typeof(JsonStringEnumConverter)), LspCompliant]
        public virtual Constants.DocumentDiagnosticReportKind.AsStringEnum Kind { get; set; }

        /// <summary>
        /// An optional result ID. If specified, will be sent with the next diagnostic report for the same document.
        /// </summary>
        [JsonPropertyName("resultId"), LspCompliant]
        public string ResultId { get; set; }

        /// <summary>
        /// The diagnostic items.
        /// </summary>
        [JsonPropertyName("items"), LspCompliant]
        public Diagnostic[] Items { get; set; }
    }
}
