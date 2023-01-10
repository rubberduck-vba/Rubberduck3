using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "inlayHintClientCapabilities")]
    public class InlayHintClientCapabilities
    {
        /// <summary>
        /// Whether the client supports dynamic registration.
        /// </summary>
        [ProtoMember(1, Name = "dynamicRegistration")]
        public bool SupportsDynamicRegistration { get; set; }

        /// <summary>
        /// Indicates which properties a client can resolve lazily on a inlay hint.
        /// </summary>
        [ProtoMember(2, Name = "resolveSupport")]
        public LazyResolutionSupport SupportsLazyResolution { get; set; }

        [ProtoContract(Name = "lazyResolutionSupport")]
        public class LazyResolutionSupport
        {
            /// <summary>
            /// The properties that a client can resolve lazily.
            /// </summary>
            [ProtoMember(1, Name = "properties")]
            public string[] Properties { get; set; }
        }
    }
}
