using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.LspServer.TextDocument.Model;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Model
{
    public class CallHierarchyIncomingCall
    {
        /// <summary>
        /// The item that makes the call.
        /// </summary>
        [JsonPropertyName("from"), LspCompliant]
        public CallHierarchyItem From { get; set; }

        /// <summary>
        /// The ranges at which the calls appear, relative to the caller denoted by the <c>From</c> item.
        /// </summary>
        [JsonPropertyName("fromRanges"), LspCompliant]
        public Range[] FromRanges { get; set; }
    }
}
