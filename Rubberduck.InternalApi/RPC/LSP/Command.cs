using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP
{
    /// <summary>
    /// Represents a reference to a command.
    /// </summary>
    public class Command
    {
        /// <summary>
        /// Display title of the command.
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; }

        /// <summary>
        /// The identifier of the actual command handler.
        /// </summary>
        [JsonPropertyName("command")]
        public string Identifier { get; set; }
    }

    /// <summary>
    /// Represents a reference to a parameterized command.
    /// </summary>
    public class ParameterizedCommand : Command
    {
        /// <summary>
        /// Arguments that the command handler should be invoked with.
        /// </summary>
        [JsonPropertyName("arguments")]
        public LSPAny[] Arguments { get; set; }
    }
}
