using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP
{
    /// <summary>
    /// Represents a related message and source code location for a diagnostic.
    /// </summary>
    /// <remarks>
    /// This should be used to point to code locations that cause or are related to a diagnostic, e.g. when duplicating a symbol in a scope.
    /// </remarks>
    public class DiagnosticRelatedInformation
    {
        /// <summary>
        /// The location of this related diagnostic information.
        /// </summary>
        [JsonPropertyName("location")]
        public Location Location { get; set; }

        /// <summary>
        /// The message of this related diagnostic information.
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
