using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    public class DocumentOnTypeFormattingOptions
    {
        /// <summary>
        /// A character on which formatting should be triggered, like '{'.
        /// </summary>
        [JsonPropertyName("firstTriggerCharacter")]
        public string FirstTriggerCharacter { get; set; }

        [JsonPropertyName("moreTriggerCharacters")]
        public string[] MoreTriggerCharacters { get; set; }
    }
}
