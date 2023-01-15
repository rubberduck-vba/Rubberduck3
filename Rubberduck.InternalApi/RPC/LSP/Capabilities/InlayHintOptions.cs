using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    public class InlayHintOptions : WorkDoneProgressOptions 
    {
        /// <summary>
        /// <c>true</c> if the server provides support to resolve additional information for inlay hints.
        /// </summary>
        [JsonPropertyName("resolveProvider")]
        public bool IsResolveProvider { get; set; }
    }
}
