using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.LspServer.Workspace.Model;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Workspace.Configuration.Options
{
    public class FileOperationRegistrationOptions
    {
        [JsonPropertyName("filters"), LspCompliant]
        public FileOperationFilter[] Filters { get; set; }
    }
}
