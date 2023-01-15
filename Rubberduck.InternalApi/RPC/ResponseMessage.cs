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


    public class SuccessResponseMessage : ResponseMessage
    {
        /// <summary>
        /// The result of a request.
        /// </summary>
        [JsonPropertyName("result")]
        public object Result { get; set; }
    }


    public class ErrorResponseMessage : ResponseMessage
    {
        /// <summary>
        /// The error object.
        /// </summary>
        [JsonPropertyName("result")]
        public ResponseError Error { get; set; }
    }
}
