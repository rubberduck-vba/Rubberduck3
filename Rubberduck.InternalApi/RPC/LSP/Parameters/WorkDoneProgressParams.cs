using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP
{
    public interface IWorkDoneProgressParams
    {
        /// <summary>
        /// A token that the server can use to report work done progress.
        /// </summary>
        [JsonPropertyName("workDoneToken")]
        string WorkDoneToken { get; set; }
    }


    public class WorkDoneProgressParams : IWorkDoneProgressParams
    {
        /// <summary>
        /// A token that the server can use to report work done progress.
        /// </summary>
        [JsonPropertyName("workDoneToken")]
        public string WorkDoneToken { get; set; }
    }
}
