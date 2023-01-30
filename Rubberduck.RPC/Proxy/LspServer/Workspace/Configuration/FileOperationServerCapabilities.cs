using Rubberduck.RPC.Platform.Metadata;
using Rubberduck.RPC.Proxy.LspServer.Workspace.Configuration.Options;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Workspace.Configuration
{
    public class FileOperationServerCapabilities
    {
        [JsonPropertyName("didCreate"), LspCompliant]
        public FileOperationRegistrationOptions DidCreate { get; set; }

        [JsonPropertyName("willCreate"), LspCompliant]
        public FileOperationRegistrationOptions WillCreate { get; set; }

        [JsonPropertyName("didRename"), LspCompliant]
        public FileOperationRegistrationOptions DidRename { get; set; }

        [JsonPropertyName("willRename"), LspCompliant]
        public FileOperationRegistrationOptions WillRename { get; set; }

        [JsonPropertyName("didDelete"), LspCompliant]
        public FileOperationRegistrationOptions DidDelete { get; set; }

        [JsonPropertyName("willDelete"), LspCompliant]
        public FileOperationRegistrationOptions WillDelete { get; set; }
    }
}
