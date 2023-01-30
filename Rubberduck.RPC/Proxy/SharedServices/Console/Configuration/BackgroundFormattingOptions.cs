using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.SharedServices.Console.Configuration
{
    public class BackgroundFormattingOptions
    {
        /// <summary>
        /// Provides the default background formatting options.
        /// </summary>
        [JsonPropertyName("defaultBackgroundProvider")]
        public ConsoleColorOptions DefaultFormatProvider { get; set; }

        /// <summary>
        /// Overrides the default background formatting configuration for the <c>Id</c> part of log messages.
        /// </summary>
        [JsonPropertyName("idBackgroundProvider")]
        public ConsoleColorOptions IdBackgroundProvider { get; set; }

        /// <summary>
        /// Overrides the default background formatting configuration for the <c>Timestamp</c> part of log messages.
        /// </summary>
        [JsonPropertyName("timestampBackgroundProvider")]
        public ConsoleColorOptions TimestampBackgroundProvider { get; set; }

        /// <summary>
        /// Overrides the default background formatting configuration for the <c>LogLevel</c> part of log messages.
        /// </summary>
        [JsonPropertyName("logLevelBackground")]
        public ConsoleColorOptions LogLevelBackgroundProvider { get; set; }

        /// <summary>
        /// Overrides the default background formatting configuration for the <c>Message</c> part of log messages.
        /// </summary>
        [JsonPropertyName("messageBackground")]
        public ConsoleColorOptions MessageBackgroundProvider { get; set; }

        /// <summary>
        /// Overrides the default background formatting configuration for the <c>Verbose</c> part of log messages.
        /// </summary>
        [JsonPropertyName("verboseBackground")]
        public ConsoleColorOptions VerboseBackgroundProvider { get; set; }
    }
}
