using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Metadata;
using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP
{
    public class WorkDoneProgressReport
    {
        [JsonPropertyName("kind"), JsonConverter(typeof(JsonStringEnumConverter)), LspCompliant]
        public Constants.LSP.WorkDoneProgressKind.AsStringEnum Kind { get; set; }
            = Constants.LSP.WorkDoneProgressKind.AsStringEnum.Report;

        /// <summary>
        /// Whether a cancel button should be shown to allow the user to cancel a long-running operation.
        /// </summary>
        [JsonPropertyName("cancellable"), LspCompliant]
        public bool Cancellable { get; set; }

        /// <summary>
        /// More detailed message associated with the progress. Contains information complementary to the title.
        /// </summary>
        /// <remarks>
        /// If <c>null</c>, the previous message (if any) is still valid.
        /// </remarks>
        [JsonPropertyName("message"), LspCompliant]
        public string Message { get; set; }

        /// <summary>
        /// Optional progress percentage to display. If not provided infinite progress is assumed.
        /// </summary>
        /// <remarks>Legal values are in the range [0..100]</remarks>
        [JsonPropertyName("percentage"), LspCompliant]
        public uint? Percentage { get; set; }
    }
}
