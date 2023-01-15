using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    public class ExecuteCommandOptions : WorkDoneProgressOptions
    {
        /// <summary>
        /// The commands to be executed on the server.
        /// </summary>
        [JsonPropertyName("commands")]
        public string[] Commands { get; set; }
    }
}
