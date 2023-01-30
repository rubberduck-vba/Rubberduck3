using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.SharedServices.Console.Commands.Parameters
{
    public class SetEnabledParams
    {
        /// <summary>
        /// The new value that should be assigned to the <c>IsEnabled</c> setting.
        /// </summary>
        [JsonPropertyName("value")]
        public bool Value { get; set; }
    }
}
