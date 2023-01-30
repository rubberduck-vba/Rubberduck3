using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.LspServer.Workspace.File.Configuration
{
    /// <summary>
    /// Options to create a file or folder.
    /// </summary>
    public class CreateFileOptions
    {
        public static readonly CreateFileOptions Default = new CreateFileOptions();

        /// <summary>
        /// Overwrites an existing file or folder.
        /// </summary>
        /// <remarks>
        /// <c>Overwrite</c> wins over <c>IgnoreIfExists</c>.
        /// </remarks>
        [JsonPropertyName("overwrite")]
        public bool Overwrite { get; set; }

        /// <summary>
        /// Ignores an already existing file or folder.
        /// </summary>
        [JsonPropertyName("ignoreIfExists")]
        public bool IgnoreIfExists { get; set; }
    }
}
