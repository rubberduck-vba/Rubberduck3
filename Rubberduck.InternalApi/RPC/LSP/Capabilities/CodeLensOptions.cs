using System.Text.Json.Serialization;
namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    public class CodeLensOptions : WorkDoneProgressOptions
    {
        /// <summary>
        /// <c>true</c> if the server provides support to resolve additional CodeLens metadata.
        /// </summary>
        [JsonPropertyName("resolveProvider")]
        public bool IsResolveProvider { get; set; }
    }
}
