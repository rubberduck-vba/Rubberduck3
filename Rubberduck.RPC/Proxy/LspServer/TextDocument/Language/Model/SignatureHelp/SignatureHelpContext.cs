using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Model
{
    public class SignatureHelpContext
    {
        [JsonPropertyName("triggerKind"), LspCompliant]
        public Constants.SignatureHelpTriggerKind.AsEnum TriggerKind { get; set; }

        /// <summary>
        /// The character that triggered signature help, if TriggerKind is 2.
        /// </summary>
        [JsonPropertyName("triggerCharacter"), LspCompliant]
        public string TriggerCharacter { get; set; }

        /// <summary>
        /// <c>true</c> if signature help was already showing when it was triggered.
        /// </summary>
        [JsonPropertyName("isRetrigger"), LspCompliant]
        public bool IsRetrigger { get; set; }

        [JsonPropertyName("activeSignatureHelp"), LspCompliant]
        public SignatureHelp ActiveSignatureHelp { get; set; }
    }
}
