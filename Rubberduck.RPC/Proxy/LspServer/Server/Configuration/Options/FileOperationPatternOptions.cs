using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Configuration.Options
{
    public class FileOperationPatternOptions
    {
        [JsonPropertyName("ignoreCase"), LspCompliant]
        public bool IgnoreCase { get; set; }
    }
}
