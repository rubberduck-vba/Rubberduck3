using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    public class FileEvent
    {
        [JsonPropertyName("uri")]
        public string Uri { get; set; }

        /// <summary>
        /// See <c>Constants.FileChangeType</c>.
        /// </summary>
        [JsonPropertyName("type")]
        public int Type { get; set; }
    }
}
