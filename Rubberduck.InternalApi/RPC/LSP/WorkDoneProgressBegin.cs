using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP
{
    public class WorkDoneProgressBegin
    {
        [JsonPropertyName("kind")]
        public string Kind { get; set; } = "begin";

        /// <summary>
        /// Mandatory title of the progress operation. Used to briefly inform about the kind of operation being performed.
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; }

        /// <summary>
        /// Whether a cancel button should be shown to allow the user to cancel a long-running operation.
        /// </summary>
        [JsonPropertyName("cancellable")]
        public bool Cancellable { get; set; }

        /// <summary>
        /// More detailed message associated with the progress. Contains information complementary to the title.
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; }

        /// <summary>
        /// Optional progress percentage to display. If not provided infinite progress is assumed.
        /// </summary>
        /// <remarks>Legal values are in the range [0..100]</remarks>
        [JsonPropertyName("percentage")]
        public uint? Percentage { get; set; }
    }
}
