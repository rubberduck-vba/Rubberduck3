using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Model
{
    public class RelatedUnchangedDocumentDiagnosticReport : RelatedFullDocumentDiagnosticReport
    {
        [JsonPropertyName("kind"), JsonConverter(typeof(JsonStringEnumConverter)), LspCompliant]
        public override Constants.DocumentDiagnosticReportKind.AsStringEnum Kind { get; set; }
    }
}
