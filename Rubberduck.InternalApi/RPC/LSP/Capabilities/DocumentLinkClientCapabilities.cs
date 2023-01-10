using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "documentLinkClientCapabilities")]
    public class DocumentLinkClientCapabilities
    {
        /// <summary>
        /// Whether the client supports dynamic registration.
        /// </summary>
        [ProtoMember(1, Name = "dynamicRegistration")]
        public bool SupportsDynamicRegistration { get; set; }

        /// <summary>
        /// Whether the client supports the 'tooltip' property on 'DocumentLink'.
        /// </summary>
        [ProtoMember(2, Name = "tooltipSupport")]
        public bool SupportsToolTip { get; set; }
    }
}
