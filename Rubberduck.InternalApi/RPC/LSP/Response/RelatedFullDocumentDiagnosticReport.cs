using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Response
{
    public class RelatedFullDocumentDiagnosticReport : FullDocumentDiagnosticReport
    {
        /// <summary>
        /// Diagnostics of related documents.
        /// </summary>
        [JsonPropertyName("relatedDocuments")]
        public Dictionary<string, FullDocumentDiagnosticReport> RelatedDocuments { get; set; }
    }
}
