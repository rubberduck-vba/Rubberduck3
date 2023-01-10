using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "signatureHelpClientCapabilities")]
    public class SignatureHelpClientCapabilities
    {
        /// <summary>
        /// Whether the client supports dynamic registration.
        /// </summary>
        [ProtoMember(1, Name = "dynamicRegistration")]
        public bool SupportsDynamicRegistration { get; set; }

        /// <summary>
        /// Describes client support for signature info.
        /// </summary>
        [ProtoMember(2, Name = "signatureInformation")]
        public SignatureInformationCapabilities SignatureInformation { get; set; }

        [ProtoMember(3, Name = "contextSupport")]
        public bool SupportsContext { get; set; }

        [ProtoContract(Name = "SignatureInformationCapabilities")]
        public class SignatureInformationCapabilities
        {
            /// <summary>
            /// Supported content formats for the 'documentation' property.
            /// </summary>
            /// <remarks>
            /// See <c>Constants.MarkupKind</c> for values.
            /// </remarks>
            [ProtoMember(1, Name = "documentationFormat")]
            public int[] DocumentationFormat { get; set; }

            /// <summary>
            /// Client capabilities specific to parameter information.
            /// </summary>
            [ProtoMember(2, Name = "parameterInformation")]
            public ParameterInformationCapabilities ParameterInformation { get; set; }

            /// <summary>
            /// <c>true</c> if the client supports the 'activeParameter' property on 'SignatureInformation' literals.
            /// </summary>
            [ProtoMember(3, Name = "activeParameterSupport")]
            public bool SupportsActiveParameter { get; set; }

            /// <summary>
            /// <c>true</c> if client supports additional context information for a 'textDocument/signatureHelp' request.
            /// </summary>
            [ProtoMember(4, Name = "contextSupport")]
            public bool SupportsContext { get; set; }

            [ProtoContract(Name = "parameterInformationCapabilities")]
            public class ParameterInformationCapabilities
            {
                /// <summary>
                /// <c>true</c> if the client supports processing label offsets instead of a simple label string.
                /// </summary>
                [ProtoMember(1, Name = "supportsLabelOffset")]
                public bool SupportsLabelOffset { get; set; }
            }
        }
    }
}
