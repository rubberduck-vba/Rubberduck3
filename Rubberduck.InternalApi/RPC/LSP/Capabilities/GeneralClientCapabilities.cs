using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "generalClientCapabilities")]
    public class GeneralClientCapabilities
    {
        /// <summary>
        /// Signals how the client handles stale requests.
        /// </summary>
        [ProtoMember(1, Name = "staleRequestSupport")]
        public StaleRequestSupportClientCapabilities StaleRequestSupport { get; set; }

        /// <summary>
        /// Client capabilities specific to regular expressions.
        /// </summary>
        [ProtoMember(2, Name = "regularExpressions")]
        public RegularExpressionClientCapabilies RegularExpressions { get; set; }

        /// <summary>
        /// Client capabilities specific to the client's markdown parser.
        /// </summary>
        [ProtoMember(3, Name = "markdown")]
        public MarkdownClientCapabilities Markdown { get; set; }

        /// <summary>
        /// Position encodings supported by the client.
        /// </summary>
        /// <remarks>
        /// See <c>Constants.PositionEncodingKind</c>
        /// </remarks>
        [ProtoMember(4, Name = "positionEncodings")]
        public string[] PositionEncodings { get; set; }

        [ProtoContract(Name = "staleRequestSupportClientCapabilities")]
        public class StaleRequestSupportClientCapabilities
        {
            /// <summary>
            /// If <c>true</c>, client will actively cancel a stale request.
            /// </summary>
            [ProtoMember(1, Name = "cancel")]
            public bool Cancel { get; set; }

            /// <summary>
            /// The list of requests for which the client will retry the request if it receives a 'ContentModified' error code in a response.
            /// </summary>
            [ProtoMember(2, Name = "retryOnContentModified")]
            public string[] RetryOnContentModified { get; set; }
        }
    }
}
