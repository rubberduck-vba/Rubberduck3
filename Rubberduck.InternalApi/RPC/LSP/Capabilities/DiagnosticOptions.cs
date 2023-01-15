using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    public class DiagnosticOptions : WorkDoneProgressOptions
    {
        /// <summary>
        /// An optional identifier under which the diagnostics are managed by the client.
        /// </summary>
        [JsonPropertyName("identifier")]
        public string Identifier { get; set; }

        /// <summary>
        /// Whether the language has inter-file dependencies.
        /// </summary>
        [JsonPropertyName("interFileDependencies")]
        public bool InterFileDependencies { get; set; } = true;

        /// <summary>
        /// Whether the server provides support for workspace diagnostics as well.
        /// </summary>
        [JsonPropertyName("workspaceDiagnostics")]
        public bool WorkspaceDiagnostics { get; set; } = true;
    }
}
