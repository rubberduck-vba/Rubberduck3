using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.SharedServices.Console.Configuration
{
    /// <summary>
    /// Configuration options for server console.
    /// </summary>
    public class ServerConsoleOptions
    {
        /// <summary>
        /// The minimum log level of messages that are output to the server console.
        /// </summary>
        [JsonPropertyName("logLevel"), JsonConverter(typeof(JsonStringEnumConverter)), RubberduckSP]
        public ServerLogLevel LogLevel { get; set; }

        /// <summary>
        /// The verbosity level of trace logs.
        /// </summary>
        [JsonPropertyName("trace"), JsonConverter(typeof(JsonStringEnumConverter)), LspCompliant]
        public Constants.Console.VerbosityOptions.AsStringEnum Trace { get; set; }

        /// <summary>
        /// Whether logging is enabled at all on this server.
        /// </summary>
        /// <remarks>
        /// Useful for pausing and resuming trace logging.
        /// </remarks>
        [JsonPropertyName("enabled")]
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Whether <c>Verbose</c> messages should be included in trace logs.
        /// </summary>
        [JsonIgnore]
        public bool IsVerbose => Trace == Constants.Console.VerbosityOptions.AsStringEnum.Verbose;

        /// <summary>
        /// Whether logging is enabled.
        /// </summary>
        /// <param name="messageLevel">The minimum level needed for a message to produce a log output.</param>
        public bool CanLog(ServerLogLevel messageLevel = ServerLogLevel.Trace) => IsEnabled 
            && Trace != Constants.Console.VerbosityOptions.AsStringEnum.Off 
            && LogLevel != ServerLogLevel.Off && messageLevel >= LogLevel;

        /// <summary>
        /// Provides formatting options for log messages output.
        /// </summary>
        [JsonPropertyName("messageFormattingProvider")]
        public ConsoleOutputFormatOptions ConsoleOutputFormatting { get; set; }
    }
}
