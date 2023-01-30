using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Server.Commands.Parameters
{
    public class ExecuteCommandParams
    {
        /// <summary>
        /// The identifier of the command to execute.
        /// </summary>
        [JsonPropertyName("command"), LspCompliant]
        public string Command { get; set; }

        /// <summary>
        /// The arguments the command should be invoked with.
        /// </summary>
        [JsonPropertyName("arguments"), LspCompliant]
        public object Arguments { get; set; }
    }

    public class ExecuteCommandParams<TParameter>
        where TParameter : class, new()
    {
        /// <summary>
        /// The identifier of the command to execute.
        /// </summary>
        [JsonPropertyName("command"), LspCompliant]
        public string Command { get; set; }

        /// <summary>
        /// The arguments the command should be invoked with.
        /// </summary>
        [JsonPropertyName("arguments"), LspCompliant]
        public TParameter Arguments { get; set; }
    }
}
