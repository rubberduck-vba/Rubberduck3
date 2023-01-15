using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    public class FileOperationServerCapabilities
    {
        [JsonPropertyName("didCreate")]
        public FileOperationRegistrationOptions DidCreate { get; set; }

        [JsonPropertyName("willCreate")]
        public FileOperationRegistrationOptions WillCreate { get; set; }

        [JsonPropertyName("didRename")]
        public FileOperationRegistrationOptions DidRename { get; set; }

        [JsonPropertyName("willRename")]
        public FileOperationRegistrationOptions WillRename { get; set; }

        [JsonPropertyName("didDelete")]
        public FileOperationRegistrationOptions DidDelete { get; set; }

        [JsonPropertyName("willDelete")]
        public FileOperationRegistrationOptions WillDelete { get; set; }
    }
}
