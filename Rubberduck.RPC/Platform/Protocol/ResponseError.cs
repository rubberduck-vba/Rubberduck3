using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Platform.Protocol
{
    public class ResponseError
    {
        /// <summary>
        /// A number indicating the type of error that occurred.
        /// </summary>
        [JsonPropertyName("code")]
        public int Code { get; set; }

        /// <summary>
        /// A string providing a short description of the error.
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; }

        /// <summary>
        /// A structured value containing additional information about the error.
        /// </summary>
        [JsonPropertyName("data")]
        public object Data { get; set; } = null;
    }
}
