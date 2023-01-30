using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Proxy.SharedServices.Console.Configuration;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.SharedServices.Console
{
    public class FontOptions
    {
        [JsonPropertyName("fontFamily")]
        public string FontFamily { get; set; } = "Consolas"; // TODO move default value somewhere else

        [JsonPropertyName("fontSize")]
        public int FontSize { get; set; } = 12; // TODO move default value somewhere else

        [JsonPropertyName("fontWeight"), JsonConverter(typeof(JsonStringEnumConverter))]
        public Constants.Console.FontWeightOptions.AsFlagsEnum FontWeight { get; set; }

        /// <summary>
        /// Provides formatting options for the foreground color.
        /// </summary>
        [JsonPropertyName("foreground")]
        public ConsoleColorOptions ForegroundColorProvider { get; set; }
    }
}
