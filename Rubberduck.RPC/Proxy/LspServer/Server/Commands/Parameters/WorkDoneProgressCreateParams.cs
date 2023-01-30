using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Server.Commands.Parameters
{
    public class WorkDoneProgressCreateParams
    {
        /// <summary>
        /// The token to be used to report progress for this task.
        /// </summary>
        [JsonPropertyName("token")]
        public string ProgressToken { get; set; }
    }
}
