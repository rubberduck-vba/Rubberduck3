using Rubberduck.RPC.Platform.Metadata;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Model
{
    public class RelatedFullDocumentDiagnosticReport : FullDocumentDiagnosticReport
    {
        /// <summary>
        /// Diagnostics of related documents.
        /// </summary>
        [JsonPropertyName("relatedDocuments"), LspCompliant]
        public Dictionary<string, FullDocumentDiagnosticReport> RelatedDocuments { get; set; }
    }
}
