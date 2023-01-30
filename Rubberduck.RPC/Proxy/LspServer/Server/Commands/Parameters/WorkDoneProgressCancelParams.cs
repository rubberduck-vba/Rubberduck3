using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Server.Commands.Parameters
{
    public class WorkDoneProgressCancelParams
    {
        /// <summary>
        /// The token to be used to report progress for this task.
        /// </summary>
        [JsonPropertyName("token"), LspCompliant]
        public string ProgressToken { get; set; }
    }
}
