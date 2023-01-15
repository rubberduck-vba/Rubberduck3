using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    public class DocumentLinkOptions : WorkDoneProgressOptions
    {
        /// <summary>
        /// <c>true</c> if the server provides support to resolve additional information for document links.
        /// </summary>
        [JsonPropertyName("resolveProvider")]
        public bool IsResolveProvider { get; set; }
    }
}
