using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Model
{
    public class CodeActionContext
    {
        /// <summary>
        /// An array of diagnostics known on the client side overlapping the range provided.
        /// </summary>
        [JsonPropertyName("diagnostics"), LspCompliant]
        public Diagnostic[] Diagnostics { get; set; }

        /// <summary>
        /// Requested kind of actions to return.
        /// </summary>
        /// <remarks>
        /// Actions not of these kinds are filtered out by the client before being shown.
        /// </remarks>
        [JsonPropertyName("only"), LspCompliant]
        public string[] Only { get; set; }

        /// <summary>
        /// The reason why code actions were requested.
        /// </summary>
        [JsonPropertyName("triggerKind"), LspCompliant]
        public Constants.CodeActionTriggerKind.AsEnum? CodeActionTriggerKind { get; set; }
    }
}
