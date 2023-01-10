using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Response
{
    public class WorkspaceFullDocumentDiagnosticReport : FullDocumentDiagnosticReport
    {
        /// <summary>
        /// The URI for which diagnostic information is reported.
        /// </summary>
        [JsonPropertyName("uri")]
        public string DocumentUri { get; set; }

        /// <summary>
        /// The version number for which the diagnostics are reported.
        /// </summary>
        /// <remarks>
        /// <c>null</c> if the document is not open.
        /// </remarks>
        [JsonPropertyName("version")]
        public int? Version { get; set; }
    }
}
