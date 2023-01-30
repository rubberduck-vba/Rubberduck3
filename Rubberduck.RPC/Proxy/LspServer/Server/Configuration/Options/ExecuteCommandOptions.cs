using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.SharedServices.WorkDoneProgress.Configuration;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Configuration.Options
{
    public class ExecuteCommandOptions : WorkDoneProgressOptions
    {
        /// <summary>
        /// The commands to be executed on the server.
        /// </summary>
        [JsonPropertyName("commands"), LspCompliant]
        public string[] Commands { get; set; }
    }
}
