using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    public class GeneralClientCapabilities
    {
        /// <summary>
        /// Signals how the client handles stale requests.
        /// </summary>
        [JsonPropertyName("staleRequestSupport")]
        public StaleRequestSupportClientCapabilities StaleRequestSupport { get; set; }

        /// <summary>
        /// Client capabilities specific to regular expressions.
        /// </summary>
        [JsonPropertyName("regularExpressions")]
        public RegularExpressionClientCapabilies RegularExpressions { get; set; }

        /// <summary>
        /// Client capabilities specific to the client's markdown parser.
        /// </summary>
        [JsonPropertyName("markdown")]
        public MarkdownClientCapabilities Markdown { get; set; }

        /// <summary>
        /// Position encodings supported by the client.
        /// </summary>
        /// <remarks>
        /// See <c>Constants.PositionEncodingKind</c>
        /// </remarks>
        [JsonPropertyName("positionEncodings")]
        public string[] PositionEncodings { get; set; }

        public class StaleRequestSupportClientCapabilities
        {
            /// <summary>
            /// If <c>true</c>, client will actively cancel a stale request.
            /// </summary>
            [JsonPropertyName("cancel")]
            public bool Cancel { get; set; }

            /// <summary>
            /// The list of requests for which the client will retry the request if it receives a 'ContentModified' error code in a response.
            /// </summary>
            [JsonPropertyName("retryOnContentModified")]
            public string[] RetryOnContentModified { get; set; }
        }
    }
}
