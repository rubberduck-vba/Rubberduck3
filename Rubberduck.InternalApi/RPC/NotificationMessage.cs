using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC
{
    public abstract class NotificationMessage : Message
    {
        /// <summary>
        /// The method to be invoked.
        /// </summary>
        [JsonPropertyName("method")]
        public string Method { get; set; }
    }

    public abstract class ParameterizedNotificationMessage : NotificationMessage
    {
        /// <summary>
        /// The method's parameter values.
        /// </summary>
        [JsonPropertyName("params")]
        public object Params { get; set; }
    }
}
