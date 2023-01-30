using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.SharedServices.Console.Commands.Parameters
{
    public class SetTraceParams
    {
        /// <summary>
        /// The new value that should be assigned to the trace setting. See <c>Constants.TraceValue</c>.
        /// </summary>
        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
}
