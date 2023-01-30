using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Workspace.File.Model
{
    /// <summary>
    /// Annotates a <c>TextEdit</c> with additional metadata.
    /// </summary>
    public class ChangeAnnotation
    {
        /// <summary>
        /// A human-readable string describing the actual change. The string is rendered prominently in the user interface.
        /// </summary>
        [JsonPropertyName("label"), LspCompliant]
        public string Label { get; set; }

        /// <summary>
        /// A flag which indicates that user confirmation is needed before applying the change.
        /// </summary>
        [JsonPropertyName("needsConfirmation"), LspCompliant]
        public bool NeedsConfirmation { get; set; }

        /// <summary>
        /// A human-readable string which is rendered less prominently in the user interface.
        /// </summary>
        [JsonPropertyName("description"), LspCompliant]
        public string Description { get; set; }
    }
}
