using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Workspace.File.Configuration
{
    /// <summary>
    /// Options to rename a file or folder.
    /// </summary>
    public class RenameFileOptions
    {
        public static readonly RenameFileOptions Default = new RenameFileOptions();

        /// <summary>
        /// Overwrites an existing file or folder if the new name is already in use.
        /// </summary>
        /// <remarks>
        /// <c>Overwrite</c> wins over <c>IgnoreIfExists</c>.
        /// </remarks>
        [JsonPropertyName("overwrite")]
        public bool Overwrite { get; set; }

        /// <summary>
        /// Operation is no-op if there is already an existing file or folder with the new name.
        /// </summary>
        [JsonPropertyName("ignoreIfExists")]
        public bool IgnoreIfExists { get; set; }
    }
}
