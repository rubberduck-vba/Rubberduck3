using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    public class FileCreate
    {
        /// <summary>
        /// A URI for the location of the file/folder being created.
        /// </summary>
        [JsonPropertyName("uri")]
        public string Uri { get; set; }
    }
}
