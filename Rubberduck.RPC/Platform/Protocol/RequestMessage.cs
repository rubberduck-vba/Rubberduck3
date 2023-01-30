using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Platform.Protocol
{
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

    public abstract class RequestMessage<TParameter> : Message
        where TParameter : class, new()
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

        /// <summary>
        /// The method's parameter values.
        /// </summary>
        [JsonPropertyName("params")]
        public TParameter Params { get; set; }
    }
}
