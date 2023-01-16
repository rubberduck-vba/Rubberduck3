using System;
using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC
{
    public class ServerConsoleOptions
    {
        [JsonPropertyName("trace")]
        public string Trace { get; set; } = Constants.TraceValue.Verbose; // TODO move default value somewhere else

        [JsonPropertyName("enabled")]
        public bool IsEnabled { get; set; } = true;

        [JsonPropertyName("messageFont")]
        public FontOptions MessageFont { get; set; }

        [JsonPropertyName("verboseFont")]
        public FontOptions VerboseFont { get; set; }

        [JsonPropertyName("messageBackground")]
        public ConsoleColorOptions MessageBackground { get; set; }
        [JsonPropertyName("verboseBackground")]
        public ConsoleColorOptions VerboseBackground { get; set; }

        public class FontOptions
        {
            [JsonPropertyName("fontFamily")]
            public string FontFamily { get; set; } = "Consolas"; // TODO move default value somewhere else

            [JsonPropertyName("fontSize")]
            public int FontSize { get; set; } = 12; // TODO move default value somewhere else

            [JsonPropertyName("fontWeight")]
            public int FontWeight { get; set; } = Constants.Console.FontWeightOptions.Normal; // TODO move default value somewhere else

            [JsonPropertyName("foreground")]
            public ConsoleColorOptions ForegroundColor { get; set; }
        }

        public class ConsoleColorOptions
        {
            [JsonPropertyName("default")]
            public ConsoleColor Default { get; set; }

            [JsonPropertyName("trace")]
            public ConsoleColor? Trace { get; set; }
            [JsonPropertyName("debug")]
            public ConsoleColor? Debug { get; set; }
            [JsonPropertyName("info")]
            public ConsoleColor? Info { get; set; }
            [JsonPropertyName("warn")]
            public ConsoleColor? Warn { get; set; }
            [JsonPropertyName("error")]
            public ConsoleColor? Error { get; set; }
        }
    }
}
