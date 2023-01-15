using System.Text.Json.Serialization;
namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    public class FileOperationFilter
    {
        [JsonPropertyName("scheme")]
        public string Scheme { get; set; }

        [JsonPropertyName("pattern")]
        public FileOperationPattern Pattern { get; set; }
    }
}
