using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Workspace.File.Model
{
    public class FileDelete
    {
        [JsonPropertyName("uri"), LspCompliant]
        public string Uri { get; set; }
    }
}
