using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.SharedServices.Console.Configuration
{
    public class FontFormattingOptions
    {
        /// <summary>
        /// The default font options that is used, unless overridden in a log section.
        /// </summary>
        [JsonPropertyName("defaultFont")]
        public FontOptions DefaultFont { get; set; }

        /// <summary>
        /// Overrides the default font options for the <c>Id</c> part of a log message.
        /// </summary>
        [JsonPropertyName("messageIdFont")]
        public FontOptions MessageIdFont { get; set; }

        /// <summary>
        /// Overrides the default font options for the <c>Timestamp</c> part of a log message.
        /// </summary>
        [JsonPropertyName("timestampFont")]
        public FontOptions TimestampFont { get; set; }

        /// <summary>
        /// Overrides the default font options for the <c>LogLevel</c> part of a log message.
        /// </summary>
        [JsonPropertyName("logLevelFont")]
        public FontOptions LogLevelFont { get; set; }

        /// <summary>
        /// Overrides the default font options for the <c>Message</c> part of a log message.
        /// </summary>

        [JsonPropertyName("messageFont")]
        public FontOptions MessageFont { get; set; }

        /// <summary>
        /// Overrides the default font options for the <c>Verbose</c> part of a log message.
        /// </summary>
        [JsonPropertyName("verboseFont")]
        public FontOptions VerboseFont { get; set; }
    }
}
