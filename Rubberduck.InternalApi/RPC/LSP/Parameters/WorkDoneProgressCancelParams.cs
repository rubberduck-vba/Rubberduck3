using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    public class WorkDoneProgressCancelParams
    {
        /// <summary>
        /// The token to be used to report progress for this task.
        /// </summary>
        [JsonPropertyName("token")]
        public string ProgressToken { get; set; }
    }
}
