using Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Model;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Workspace.Language.Model
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
