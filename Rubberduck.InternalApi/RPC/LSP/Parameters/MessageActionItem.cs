using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    public class MessageActionItem
    {
        /// <summary>
        /// A short title like 'Retry', 'Open Log', etc.
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; }
    }
}
