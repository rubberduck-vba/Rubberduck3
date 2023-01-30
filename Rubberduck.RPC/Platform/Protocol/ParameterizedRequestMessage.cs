using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Platform.Protocol
{
    public abstract class ParameterizedRequestMessage : RequestMessage
    {
        /// <summary>
        /// The method's parameter values.
        /// </summary>
        [JsonPropertyName("params")]
        public object Params { get; set; }
    }
}
