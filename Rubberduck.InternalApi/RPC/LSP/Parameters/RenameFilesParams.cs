using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    public class RenameFilesParams
    {
        /// <summary>
        /// The files/folders renamed in this operation.
        /// </summary>
        [JsonPropertyName("files")]
        public FileRename[] Files { get; set; }
    }
}
