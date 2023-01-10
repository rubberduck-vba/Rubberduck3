using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC
{
    public class ProgressNotification : ParameterizedNotificationMessage
    {
        public ProgressNotification(string progressToken, int value)
        {
            Method = "$/progress";
            Params = new ProgressParams { Token = progressToken, Value = value };
        }
    }

    public class ProgressParams
    {
        /// <summary>
        /// The progress token provided by the client or server.
        /// </summary>
        [JsonPropertyName("token")]
        public string Token { get; set; }

        /// <summary>
        /// The progress data.
        /// </summary>
        [JsonPropertyName("value")]
        public int Value { get; set; }
    }
}
