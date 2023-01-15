using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    public class CodeActionContext
    {
        /// <summary>
        /// An array of diagnostics known on the client side overlapping the range provided.
        /// </summary>
        [JsonPropertyName("diagnostics")]
        public Diagnostic[] Diagnostics { get; set; }

        /// <summary>
        /// Requested kind of actions to return.
        /// </summary>
        /// <remarks>
        /// Actions not of these kinds are filtered out by the client before being shown.
        /// </remarks>
        [JsonPropertyName("only")]
        public string[] Only { get; set; }

        /// <summary>
        /// The reason why code actions were requested.
        /// </summary>
        [JsonPropertyName("triggerKind")]
        public int? CodeActionTriggerKind { get; set; } = Constants.CodeActionTriggerKind.Invoked;
    }
}
