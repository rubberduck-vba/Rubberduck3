using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    public class ExecuteCommandParams
    {
        /// <summary>
        /// The identifier of the command to execute.
        /// </summary>
        [JsonPropertyName("command")]
        public string Command { get; set; }

        /// <summary>
        /// The arguments the command should be invoked with.
        /// </summary>
        [JsonPropertyName("arguments")]
        public LSPAny[] Arguments { get; set; }
    }
}
