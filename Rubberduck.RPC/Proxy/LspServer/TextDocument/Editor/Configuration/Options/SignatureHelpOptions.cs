using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.SharedServices.WorkDoneProgress.Configuration;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Editor.Configuration
{
    public class SignatureHelpOptions : WorkDoneProgressOptions
    {
        /// <summary>
        /// The characters that trigger signature help automatically.
        /// </summary>
        [JsonPropertyName("triggerCharacters"), LspCompliant]
        public string[] TriggerCharacters { get; set; }

        /// <summary>
        /// List of characters that re-trigger signature help.
        /// </summary>
        /// <remarks>
        /// These trigger characters are only active when signature help is already showing. 
        /// All trigger characters are also re-trigger characters.
        /// </remarks>
        [JsonPropertyName("retriggerCharacters"), LspCompliant]
        public string[] RetriggerCharacters { get; set; }
    }
}
