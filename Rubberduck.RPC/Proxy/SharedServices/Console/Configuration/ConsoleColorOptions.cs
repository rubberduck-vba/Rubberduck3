using System;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.SharedServices.Console.Configuration
{
    public class ConsoleColorOptions
    {
        /// <summary>
        /// The default color that is used, unless overridden by a non-null <c>ServerLogLevel</c> configuration.
        /// </summary>
        [JsonPropertyName("default")]
        public ConsoleColor Default { get; set; }

        /// <summary>
        /// Overrides the default color for <c>Trace</c> messages.
        /// </summary>
        [JsonPropertyName("trace")]
        public ConsoleColor? Trace { get; set; }

        /// <summary>
        /// Overrides the default color for <c>Debug</c> messages.
        /// </summary>
        [JsonPropertyName("debug")]
        public ConsoleColor? Debug { get; set; }

        /// <summary>
        /// Overrides the default color for <c>Info</c> messages.
        /// </summary>
        [JsonPropertyName("info")]
        public ConsoleColor? Info { get; set; }

        /// <summary>
        /// Overrides the default color for <c>Warn</c> messages.
        /// </summary>
        [JsonPropertyName("warn")]
        public ConsoleColor? Warn { get; set; }

        /// <summary>
        /// Overrides the default color for <c>Errorr</c> messages.
        /// </summary>
        [JsonPropertyName("error")]
        public ConsoleColor? Error { get; set; }

        /// <summary>
        /// Overrides the default color for <c>Fatal</c> messages.
        /// </summary>
        [JsonPropertyName("fatal")]
        public ConsoleColor? Fatal { get; set; }

        /// <summary>
        /// Gets the configured <c>ConsoleColor</c> value for a specified <c>ServerLogLevel</c>.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public ConsoleColor For(ServerLogLevel level)
        {
            ConsoleColor color;
            switch (level)
            {
                case ServerLogLevel.Trace:
                    color = Trace ?? Default;
                    break;
                case ServerLogLevel.Debug:
                    color = Debug ?? Default;
                    break;
                case ServerLogLevel.Info:
                    color = Info ?? Default;
                    break;
                case ServerLogLevel.Warn:
                    color = Warn ?? Default;
                    break;
                case ServerLogLevel.Error:
                    color = Error ?? Default;
                    break;
                case ServerLogLevel.Fatal:
                    color = Fatal ?? Default;
                    break;
                default:
                    color = Default;
                    break;
            }

            return color;
        }
    }
}
