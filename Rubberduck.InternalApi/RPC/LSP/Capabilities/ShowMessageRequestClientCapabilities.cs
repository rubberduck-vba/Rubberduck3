using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "showMessageRequestClientCapabilities")]
    public class ShowMessageRequestClientCapabilities
    {
        /// <summary>
        /// Capabilities specific to the 'MessageActionItem' type.
        /// </summary>
        [ProtoMember(1, Name = "messageActionItem")]
        public MessageActionItemCapabilities MessageActionItem { get; set; }

        [ProtoContract(Name = "messageActionItemCapabilities")]
        public class MessageActionItemCapabilities
        {
            /// <summary>
            /// Whether the client supports additional attributes that are preserved and sent back to the server in the response.
            /// </summary>
            [ProtoMember(1, Name = "additionalPropertiesSupport")]
            public bool SupportsAdditionalProperties { get; set; }
        }
    }
}
