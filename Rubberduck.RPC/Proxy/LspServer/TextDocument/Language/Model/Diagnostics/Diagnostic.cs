using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.LspServer.TextDocument.Model;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Model
{
    /// <summary>
    /// Represents a diagnostic, such as a compiler error or warning.
    /// </summary>
    public class Diagnostic
    {
        /// <summary>
        /// The range at which the message applies.
        /// </summary>
        [JsonPropertyName("range"), LspCompliant]
        public Range Range { get; set; }

        /// <summary>
        /// The diagnostic's severity.
        /// </summary>
        /// <remarks>
        /// See <c>DiagnosticSeverity</c> constants.
        /// </remarks>
        [JsonPropertyName("severity"), LspCompliant]
        public Constants.DiagnosticSeverity.AsEnum Severity { get; set; }

        /// <summary>
        /// The diagnostic's code, which might appear in the user interface.
        /// </summary>
        [JsonPropertyName("code"), LspCompliant]
        public string Code { get; set; }

        /// <summary>
        /// Describes the error code.
        /// </summary>
        [JsonPropertyName("codeDescription"), LspCompliant]
        public string CodeDescription { get; set; }

        /// <summary>
        /// A human-readable string describing the source of this diagnostic.
        /// </summary>
        [JsonPropertyName("source"), LspCompliant]
        public string Source { get; set; } = "Rubberduck";

        /// <summary>
        /// The diagnostic's message.
        /// </summary>
        [JsonPropertyName("message"), LspCompliant]
        public string Message { get; set; }

        /// <summary>
        /// Additional metadata about the diagnostic.
        /// </summary>
        /// <remarks>See <c>DiagnosticTags</c> constants.</remarks>
        [JsonPropertyName("tags"), LspCompliant]
        public Constants.DiagnosticTags.AsEnum[] Tags { get; set; }

        /// <summary>
        /// An array of related diagnostic information.
        /// </summary>
        [JsonPropertyName("relatedInformation"), LspCompliant]
        public DiagnosticRelatedInformation[] RelatedInformation { get; set; }

        /// <summary>
        /// A data entry field that is preserved between a `textDocument/publishDiagnostics` notification and a subsequent `textDocument/codeAction` request.
        /// </summary>
        [JsonPropertyName("data"), LspCompliant]
        public string Data { get; set; }
    }
}
