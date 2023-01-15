using Rubberduck.InternalApi.RPC.LSP.Response;
using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    public class SignatureHelpContext
    {
        [JsonPropertyName("triggerKind")]
        public int TriggerKind { get; set; } = Constants.SignatureHelpTriggerKind.Invoked;

        /// <summary>
        /// The character that triggered signature help, if TriggerKind is 2.
        /// </summary>
        [JsonPropertyName("triggerCharacter")]
        public string TriggerCharacter { get; set; }

        /// <summary>
        /// <c>true</c> if signature help was already showing when it was triggered.
        /// </summary>
        [JsonPropertyName("isRetrigger")]
        public bool IsRetrigger { get; set; }

        [JsonPropertyName("activeSignatureHelp")]
        public SignatureHelp ActiveSignatureHelp { get; set; }
    }
}
