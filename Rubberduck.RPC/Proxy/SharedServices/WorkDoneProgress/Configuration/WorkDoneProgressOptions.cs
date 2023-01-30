using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Proxy.SharedServices.WorkDoneProgress.Configuration
{
    public abstract class WorkDoneProgressOptions
    {
        [JsonPropertyName("workDoneProgress")]
        public bool WorkDoneProgress { get; set; }
    }
}
