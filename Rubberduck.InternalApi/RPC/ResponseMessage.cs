using ProtoBuf;
using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC
{
    public abstract class ResponseMessage : Message
    {
        /// <summary>
        /// The request ID.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }

    [ProtoContract]
    public class SuccessResponseMessage : ResponseMessage
    {
        /// <summary>
        /// The result of a request.
        /// </summary>
        [JsonPropertyName("result")]
        [ProtoMember(1)]
        public object Result { get; set; }
    }

    [ProtoContract]
    public class ErrorResponseMessage : ResponseMessage
    {
        /// <summary>
        /// The error object.
        /// </summary>
        [JsonPropertyName("result")]
        [ProtoMember(1)]
        public ResponseError Error { get; set; }
    }
}
