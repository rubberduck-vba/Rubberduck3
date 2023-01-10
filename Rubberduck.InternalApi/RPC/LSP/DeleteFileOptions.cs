using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP
{
    /// <summary>
    /// Options to delete a file or folder.
    /// </summary>
    public class DeleteFileOptions
    {
        public static readonly DeleteFileOptions Default = new DeleteFileOptions();

        [JsonPropertyName("recursive")]
        public bool Recursive { get; set; }

        /// <summary>
        /// Ignores an already non-existing file or folder.
        /// </summary>
        [JsonPropertyName("ignoreIfNotExists")]
        public bool IgnoreIfNotExists { get; set; }
    }
}
