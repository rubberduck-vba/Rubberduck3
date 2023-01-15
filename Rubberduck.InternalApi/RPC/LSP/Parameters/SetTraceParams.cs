using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    public class SetTraceParams
    {
        /// <summary>
        /// The new value that should be assigned to the trace setting. See <c>Constants.TraceValue</c>.
        /// </summary>
        [JsonPropertyName("value")]
        public string Value { get; set; }
    }

    public class LogTraceParams
    {
        /// <summary>
        /// The message to be logged.
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; }
    }

    public class VerboseLogTraceParams : LogTraceParams
    {
        /// <summary>
        /// Additional information that can be computed when the trace level is set to 'verbose'.
        /// </summary>
        [JsonPropertyName("verbose")]
        public string Verbose { get; set; }
    }
}
