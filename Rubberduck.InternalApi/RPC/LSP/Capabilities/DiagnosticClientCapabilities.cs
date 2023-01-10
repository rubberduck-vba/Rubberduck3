using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "diagnosticClientCapabilities")]
    public class DiagnosticClientCapabilities
    {
        /// <summary>
        /// Whether the client supports dynamic registration.
        /// </summary>
        [ProtoMember(1, Name = "dynamicRegistration")]
        public bool SupportsDynamicRegistration { get; set; }

        /// <summary>
        /// Whether the client supports related documents for document diagnostic pulls.
        /// </summary>
        [ProtoMember(2, Name = "relatedDocumentSupport")]
        public bool SupportsRelatedDocument { get; set; }
    }
}
