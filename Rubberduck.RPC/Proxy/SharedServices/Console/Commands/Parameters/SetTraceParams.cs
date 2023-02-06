using System.Text.Json.Serialization;
using static Rubberduck.RPC.Platform.Constants.Console;

namespace Rubberduck.RPC.Proxy.SharedServices.Console.Commands.Parameters
{
    public class SetTraceParams
    {
        /// <summary>
        /// The new value that should be assigned to the trace setting. See <c>Constants.TraceValue</c>.
        /// </summary>
        [JsonPropertyName("value"), JsonConverter(typeof(JsonStringEnumConverter))]
        public VerbosityOptions.AsStringEnum Value { get; set; }
    }
}
