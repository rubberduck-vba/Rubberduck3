using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    public class ShowMessageRequestClientCapabilities
    {
        /// <summary>
        /// Capabilities specific to the 'MessageActionItem' type.
        /// </summary>
        [JsonPropertyName("messageActionItem")]
        public MessageActionItemCapabilities MessageActionItem { get; set; }

        public class MessageActionItemCapabilities
        {
            /// <summary>
            /// Whether the client supports additional attributes that are preserved and sent back to the server in the response.
            /// </summary>
            [JsonPropertyName("additionalPropertiesSupport")]
            public bool SupportsAdditionalProperties { get; set; }
        }
    }
}
