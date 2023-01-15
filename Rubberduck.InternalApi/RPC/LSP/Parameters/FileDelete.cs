using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    public class FileDelete
    {
        [JsonPropertyName("uri")]
        public string Uri { get; set; }
    }
}
