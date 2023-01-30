using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Workspace.Model
{
    public class FileOperationFilter
    {
        [JsonPropertyName("scheme"), LspCompliant]
        public string Scheme { get; set; }

        [JsonPropertyName("pattern"), LspCompliant]
        public FileOperationPattern Pattern { get; set; }
    }
}
