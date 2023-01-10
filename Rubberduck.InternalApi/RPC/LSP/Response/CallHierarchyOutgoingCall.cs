using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Response
{
    public class CallHierarchyOutgoingCall
    {
        /// <summary>
        /// The item that is called.
        /// </summary>
        [JsonPropertyName("to")]
        public CallHierarchyItem To { get; set; }

        /// <summary>
        /// The ranges at which this item is called. This is the range relative to the caller, i.e. the item passed to the 'callHierarchy/outgoingCalls' request.
        /// </summary>
        [JsonPropertyName("fromRanges")]
        public Range[] FromRanges { get; set; }
    }
}
