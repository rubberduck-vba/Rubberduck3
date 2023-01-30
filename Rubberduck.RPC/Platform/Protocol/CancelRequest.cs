using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Platform.Protocol
{
    public class CancelRequest : ParameterizedNotificationMessage
    {
        public CancelRequest(string requestId)
        {
            Method = "$/cancelRequest";
            Params = new CancelParams { Id = requestId };
        }

    }

    public class CancelParams
    {
        /// <summary>
        /// The request ID to cancel.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}
