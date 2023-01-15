using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    public class CreateFilesParams
    {
        /// <summary>
        /// An array of all files/folders created in this operation.
        /// </summary>
        [JsonPropertyName("files")]
        public FileCreate[] Files { get; set; }
    }
}
