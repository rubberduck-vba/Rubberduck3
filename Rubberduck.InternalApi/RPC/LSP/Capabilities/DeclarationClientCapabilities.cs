using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "declarationClientCapabilities")]
    public class DeclarationClientCapabilities
    {
        /// <summary>
        /// Whether the client supports dynamic registration.
        /// </summary>
        [ProtoMember(1, Name = "dynamicRegistration")]
        public bool SupportsDynamicRegistration { get; set; }

        /// <summary>
        /// Whether the client supports additional metadata in the form of declaration links.
        /// </summary>
        [ProtoMember(2, Name = "linkSupport")]
        public bool SupportsLinks { get; set; }
    }
}
