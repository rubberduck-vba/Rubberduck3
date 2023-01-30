using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.SharedServices.Console.Configuration
{
    public class ConsoleOutputFormatOptions
    {
        /// <summary>
        /// A character or string that separates message parts in the output.
        /// </summary>
        [JsonPropertyName("separator")]
        public string MessagePartSeparator { get; set; } = " ";

        /// <summary>
        /// A .net format string for the <c>Timestamp</c> part of log messages.
        /// </summary>
        [JsonPropertyName("timestampFormat")]
        public string TimestampFormatString { get; set; } = "yyyy-MM-dd hh:mm:ss.fff";

        /// <summary>
        /// Provides formatting options for fonts and individual log message sections.
        /// </summary>
        [JsonPropertyName("fontFormatProvider")]
        public FontFormattingOptions FontFormatting { get; set; }

        /// <summary>
        /// Provides formatting options for the background color of individual log message sections.
        /// </summary>
        [JsonPropertyName("backgroundFormatProvider")]
        public BackgroundFormattingOptions BackgroundFormatting { get; set; }
    }
}
