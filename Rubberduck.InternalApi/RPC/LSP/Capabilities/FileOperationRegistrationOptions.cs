using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    public class FileOperationRegistrationOptions
    {
        [JsonPropertyName("filters")]
        public FileOperationFilter[] Filters { get; set; }
    }
}
