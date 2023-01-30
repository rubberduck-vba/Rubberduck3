using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.SharedServices.WorkDoneProgress.Configuration;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.TextDocument.Editor.Configuration
{
    public class DocumentOnTypeFormattingOptions : WorkDoneProgressOptions
    {
        /// <summary>
        /// A character on which formatting should be triggered, like '{'.
        /// </summary>
        [JsonPropertyName("firstTriggerCharacter"), LspCompliant]
        public string FirstTriggerCharacter { get; set; }

        [JsonPropertyName("moreTriggerCharacters"), LspCompliant]
        public string[] MoreTriggerCharacters { get; set; }
    }
}
