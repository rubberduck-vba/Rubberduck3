using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Workspace.File.Model
{
    public class FileEvent
    {
        [JsonPropertyName("uri"), LspCompliant]
        public string Uri { get; set; }

        [JsonPropertyName("type"), JsonConverter(typeof(JsonStringEnumConverter)), LspCompliant]
        public Constants.FileChangeType.AsEnum Type { get; set; }
    }
}
