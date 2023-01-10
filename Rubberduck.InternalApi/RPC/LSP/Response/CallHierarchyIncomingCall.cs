using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Response
{
    public class CallHierarchyIncomingCall
    {
        /// <summary>
        /// The item that makes the call.
        /// </summary>
        [JsonPropertyName("from")]
        public CallHierarchyItem From { get; set; }

        /// <summary>
        /// The ranges at which the calls appear, relative to the caller denoted by the <c>From</c> item.
        /// </summary>
        [JsonPropertyName("fromRanges")]
        public Range[] FromRanges { get; set; }
    }
}
