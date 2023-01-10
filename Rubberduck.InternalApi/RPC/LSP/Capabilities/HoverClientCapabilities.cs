using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "hoverClientCapabilities")]
    public class HoverClientCapabilities
    {
        /// <summary>
        /// Whether the client supports dynamic registration.
        /// </summary>
        [ProtoMember(1, Name = "dynamicRegistration")]
        public bool SupportsDynamicRegistration { get; set; }

        /// <summary>
        /// See <c>MarkupKind</c> constants.
        /// </summary>
        [ProtoMember(2, Name = "contentFormat")]
        public int[] ContentFormat { get; set; }
    }
}
