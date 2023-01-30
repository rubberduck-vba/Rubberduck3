using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Workspace.Model
{
    /// <summary>
    /// Represents a reference to a command.
    /// </summary>
    public class Command
    {
        /// <summary>
        /// Display title of the command.
        /// </summary>
        [JsonPropertyName("title"), LspCompliant]
        public string Title { get; set; }

        /// <summary>
        /// The identifier of the actual command handler.
        /// </summary>
        [JsonPropertyName("command"), LspCompliant]
        public string Identifier { get; set; }
    }

    /// <summary>
    /// Represents a reference to a parameterized command.
    /// </summary>
    public class Command<TParameter> : Command
        where TParameter : class, new()
    {
        /// <summary>
        /// Arguments that the command handler should be invoked with.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        [JsonPropertyName("arguments"), LspCompliant]
        public TParameter[] Arguments { get; set; }
    }
}
