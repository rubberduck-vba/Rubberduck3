using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "codeActionClientCapabilities")]
    public class CodeActionClientCapabilities
    {
        /// <summary>
        /// Whether the client supports dynamic registration.
        /// </summary>
        [ProtoMember(1, Name = "dynamicRegistration")]
        public bool SupportsDynamicRegistration { get; set; }

        /// <summary>
        /// If supplied, the client supports code action literals as a valid response of the 'textDocument/codeAction' request.
        /// </summary>
        [ProtoMember(2, Name = "codeActionLiteralSupport")]
        public CodeActionLiteralSupport SupportsCodeActionLiteral { get; set; }

        /// <summary>
        /// Whether the client supports the 'isPreferred' code action property.
        /// </summary>
        [ProtoMember(3, Name = "isPreferredSupport")]
        public bool SupportsIsPreferred { get; set; }

        /// <summary>
        /// Whether the client supports the 'isDisabled' code action property.
        /// </summary>
        [ProtoMember(4, Name = "isDisabledSupport")]
        public bool SupportsIsDisabled { get; set; }

        /// <summary>
        /// Whether the client supports the 'data' code action property.
        /// </summary>
        /// <remarks>
        /// If supported, the data is preserved between a 'textDocument/codeAction' and a 'codeAction/resolve' request.
        /// </remarks>
        [ProtoMember(5, Name = "dataSupport")]
        public bool SupportsData { get; set; }

        /// <summary>
        /// Whether the client supports resolving additional code action properties via a separate 'codeAction/resolve' request.
        /// </summary>
        [ProtoMember(6, Name = "resolveSupport")]
        public PropertyResolutionSupport SupportsPropertyResolution { get; set; }

        /// <summary>
        /// Whether the client honors the change annotations in text edits and resource operations returned via the 'CodeAction#edit' property.
        /// </summary>
        [ProtoMember(7, Name = "honorsChangeAnnotations")]
        public bool HonorsChangeAnnotations { get; set; }

        [ProtoContract(Name = "propertyResolutionSupport")]
        public class PropertyResolutionSupport
        {
            /// <summary>
            /// The properties that a client can resolve lazily.
            /// </summary>
            [ProtoMember(1, Name = "properties")]
            public string[] Properties { get; set; }
        }

        [ProtoContract(Name = "codeActionLiteralSupport")]
        public class CodeActionLiteralSupport
        {
            [ProtoMember(1, Name = "codeActionKind")]
            public CodeActionKindSupport CodeActionKind { get; set; }

            [ProtoContract(Name = "codeActionKindSupport")]
            public class CodeActionKindSupport
            {
                /// <summary>
                /// The <c>Constants.CodeActionKind</c> values supported by the client.
                /// </summary>
                [ProtoMember(1, Name = "valueSet")]
                public string[] ValueSet { get; set; }
            }
        }
    }
}
