using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Workspace.Configuration.Client
{
    public class GeneralClientCapabilities
    {
        /// <summary>
        /// Signals how the client handles stale requests.
        /// </summary>
        [JsonPropertyName("staleRequestSupport"), LspCompliant]
        public StaleRequestSupportClientCapabilities StaleRequestSupport { get; set; }

        /// <summary>
        /// Client capabilities specific to regular expressions.
        /// </summary>
        [JsonPropertyName("regularExpressions"), LspCompliant]
        public RegularExpressionClientCapabilies RegularExpressions { get; set; }

        /// <summary>
        /// Client capabilities specific to the client's markdown parser.
        /// </summary>
        [JsonPropertyName("markdown"), LspCompliant]
        public MarkdownClientCapabilities Markdown { get; set; }

        /// <summary>
        /// Position encodings supported by the client.
        /// </summary>
        /// <remarks>
        /// See <c>Constants.PositionEncodingKind</c>
        /// </remarks>
        [JsonPropertyName("positionEncodings"), LspCompliant]
        public string[] PositionEncodings { get; set; }

        public class RegularExpressionClientCapabilies
        {
            [JsonPropertyName("engine"), LspCompliant]
            public string Engine { get; set; }

            [JsonPropertyName("version"), LspCompliant]
            public string Version { get; set; }
        }

        public class MarkdownClientCapabilities
        {
            /// <summary>
            /// The name of the Markdown parser.
            /// </summary>
            [JsonPropertyName("parser"), LspCompliant]
            public string Parser { get; set; }

            /// <summary>
            /// The parser version.
            /// </summary>
            [JsonPropertyName("version"), LspCompliant]
            public string Version { get; set; }

            /// <summary>
            /// A list of HTML tags that the client allows / supports in Markdown.
            /// </summary>
            [JsonPropertyName("allowedTags"), LspCompliant]
            public string[] AllowedHtmlTags { get; set; }
        }

        public class StaleRequestSupportClientCapabilities
        {
            /// <summary>
            /// If <c>true</c>, client will actively cancel a stale request.
            /// </summary>
            [JsonPropertyName("cancel"), LspCompliant]
            public bool Cancel { get; set; }

            /// <summary>
            /// The list of requests for which the client will retry the request if it receives a 'ContentModified' error code in a response.
            /// </summary>
            [JsonPropertyName("retryOnContentModified"), LspCompliant]
            public string[] RetryOnContentModified { get; set; }
        }
    }
}
