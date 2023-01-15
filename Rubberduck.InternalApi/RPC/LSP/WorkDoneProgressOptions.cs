using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP
{
    public class WorkDoneProgressOptions
    {
        [JsonPropertyName("workDoneProgress")]
        public bool WorkDoneProgress { get; set; }
    }
}
