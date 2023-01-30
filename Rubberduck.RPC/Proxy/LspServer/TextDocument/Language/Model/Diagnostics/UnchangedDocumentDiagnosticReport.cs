using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Model
{
    /// <summary>
    /// A diagnostic report indicating that the previous report is still accurate.
    /// </summary>
    public class UnchangedDocumentDiagnosticReport : FullDocumentDiagnosticReport
    {
        [JsonPropertyName("kind"), JsonConverter(typeof(JsonStringEnumConverter)), LspCompliant]
        public override Constants.DocumentDiagnosticReportKind.AsStringEnum Kind { get; set; }
    }
}
