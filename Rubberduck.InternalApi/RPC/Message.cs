using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC
{
    public abstract class Message
    {
        /// <summary>
        /// A string specifying the version of the JSON-RPC protocol. MUST be exactly "2.0".
        /// </summary>
        [JsonPropertyName("jsonrpc")]
        public string JsonRpc { get; set; } = "2.0";
    }

    public abstract class RequestMessage : Message
    {
        /// <summary>
        /// The request ID.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }
        /// <summary>
        /// The method to be invoked.
        /// </summary>
        [JsonPropertyName("method")]
        public string Method { get; set; }
    }

    public abstract class ParameterizedRequestMessage : RequestMessage
    {
        /// <summary>
        /// The method's parameter values.
        /// </summary>
        [JsonPropertyName("params")]
        public object Params { get; set; }
    }
}
