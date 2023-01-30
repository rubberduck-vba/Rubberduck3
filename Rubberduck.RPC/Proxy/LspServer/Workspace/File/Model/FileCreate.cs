using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Workspace.File.Model
{
    public class FileCreate
    {
        /// <summary>
        /// A URI for the location of the file/folder being created.
        /// </summary>
        [JsonPropertyName("uri"), LspCompliant]
        public string Uri { get; set; }
    }
}
