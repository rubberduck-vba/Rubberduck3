using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    public class DeleteFilesParams
    {
        /// <summary>
        /// The files/folders deleted in this operation.
        /// </summary>
        [JsonPropertyName("files")]
        public FileDelete[] Files { get; set; }
    }
}
