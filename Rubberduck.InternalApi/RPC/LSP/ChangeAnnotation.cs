using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP
{
    /// <summary>
    /// Annotates a <c>TextEdit</c> with additional metadata.
    /// </summary>
    public class ChangeAnnotation
    {
        /// <summary>
        /// A human-readable string describing the actual change. The string is rendered prominently in the user interface.
        /// </summary>
        [JsonPropertyName("label")]
        public string Label { get; set; }

        /// <summary>
        /// A flag which indicates that user confirmation is needed before applying the change.
        /// </summary>
        [JsonPropertyName("needsConfirmation")]
        public bool NeedsConfirmation { get; set; }

        /// <summary>
        /// A human-readable string which is rendered less prominently in the user interface.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}
