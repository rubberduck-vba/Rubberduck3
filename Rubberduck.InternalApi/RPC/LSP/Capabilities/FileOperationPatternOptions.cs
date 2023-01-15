using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    public class FileOperationPatternOptions
    {
        [JsonPropertyName("ignoreCase")]
        public bool IgnoreCase { get; set; }
    }
}
