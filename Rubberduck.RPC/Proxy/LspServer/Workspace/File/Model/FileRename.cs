using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Workspace.File.Model
{
    public class FileRename
    {
        /// <summary>
        /// The old/current URI for the location of the file/folder being renamed.
        /// </summary>
        [JsonPropertyName("oldUri"), LspCompliant]
        public string OldUri { get; set; }

        /// <summary>
        /// The URI of the new location of the file/folder being renamed.
        /// </summary>
        [JsonPropertyName("newUri"), LspCompliant]
        public string NewUri { get; set; }
    }
}
