using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Server.Model
{
    public class WorkDoneProgressEnd
    {
        [JsonPropertyName("kind"), JsonConverter(typeof(JsonStringEnumConverter)), LspCompliant]
        public Constants.LSP.WorkDoneProgressKind.AsStringEnum Kind { get; set; }
            = Constants.LSP.WorkDoneProgressKind.AsStringEnum.End;

        /// <summary>
        /// A final message indicating the outcome of the operation.
        /// </summary>
        [JsonPropertyName("message"), LspCompliant]
        public string Message { get; set; }
    }
}
