using System.Text.Json.Serialization;
namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    public class DidChangeWatchedFilesParams
    {
        [JsonPropertyName("changes")]
        public FileEvent[] Changes { get; set; }
    }
}
