using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "renameClientCapabilities")]
    public class RenameClientCapabilities
    {
        /// <summary>
        /// Whether the client supports dynamic registration.
        /// </summary>
        [ProtoMember(1, Name = "dynamicRegistration")]
        public bool SupportsDynamicRegistration { get; set; }

        /// <summary>
        /// Whether client supports testing for validity of rename operations before execution.
        /// </summary>
        [ProtoMember(2, Name = "prepareSupport")]
        public bool SupportsPreparation { get; set; }

        /// <summary>
        /// See <c>Constants.PrepareSupportDefaultBehavior</c>.
        /// </summary>
        [ProtoMember(3, Name = "prepareSupportDefaultBehavior")]
        public int PrepareSupportDefaultBehavior { get; set; } = Constants.PrepareSupportDefaultBehavior.Identifier;

        /// <summary>
        /// Whether the client honors the change annotations in text edits and resource operations returned via the rename request's workspace edit.
        /// </summary>
        [ProtoMember(4, Name = "honorsChangeAnnotations")]
        public bool HonorsChangeAnnotations { get; set; }
    }
}
