using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.LspServer.TextDocument.Model;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Language.Model
{
    public class CallHierarchyOutgoingCall
    {
        /// <summary>
        /// The item that is called.
        /// </summary>
        [JsonPropertyName("to"), LspCompliant]
        public CallHierarchyItem To { get; set; }

        /// <summary>
        /// The ranges at which this item is called. This is the range relative to the caller, i.e. the item passed to the 'callHierarchy/outgoingCalls' request.
        /// </summary>
        [JsonPropertyName("fromRanges"), LspCompliant]
        public Range[] FromRanges { get; set; }
    }
}
