

using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    public class CodeActionOptions : WorkDoneProgressOptions
    {
        /// <summary>
        /// CodeActionKinds that this server may return.
        /// </summary>
        [JsonPropertyName("codeActionKinds")]
        public string[] CodeActionKinds { get; set; }

        /// <summary>
        /// <c>true</c> if the server provides support to resolve additional information for a code action.
        /// </summary>
        [JsonPropertyName("resolveProvider")]
        public bool IsResolveProvider { get; set; }
    }
}
