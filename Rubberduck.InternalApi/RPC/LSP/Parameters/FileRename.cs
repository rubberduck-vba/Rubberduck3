using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    public class FileRename
    {
        /// <summary>
        /// The old/current URI for the location of the file/folder being renamed.
        /// </summary>
        [JsonPropertyName("oldUri")]
        public string OldUri { get; set; }

        /// <summary>
        /// The URI of the new location of the file/folder being renamed.
        /// </summary>
        [JsonPropertyName("newUri")]
        public string NewUri { get; set; }
    }
}
