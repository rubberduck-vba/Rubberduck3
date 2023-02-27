using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Platform.Model.Telemetry
{
    /// <summary>
    /// <strong>RequestTelemetry</strong> represents the logical sequence of execution triggered by an external request to your application.
    /// Every request execution is identified by unique ID and URL containing all the execution parameters.
    /// RequestsTelemetry items may be regrouped by logical name and define the source of this request.
    /// Code execution may fail or succeed, and has a certain duration.
    /// </summary>
    public class RequestTelemetry : TelemetryEvent
    {
        public RequestTelemetry() : base(TelemetryEventName.Request) { }

        /// <summary>
        /// Name (URI) of the request, represents the code path taken to process the request.
        /// </summary>
        /// <remarks>
        /// Maximum value length: 1,024
        /// </remarks>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Identifies an instance of a request call; correlates between request and other telemetry items.
        /// </summary>
        /// <remarks>
        /// Value should be globally unique. Maximum length: 128
        /// </remarks>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// Request URL with all query string parameters.
        /// </summary>
        /// <remarks>
        /// Maximum lenght: 2,048
        /// </remarks>
        [JsonPropertyName("url")]
        public string Url { get; set; }

        /// <summary>
        /// Source of the request.
        /// </summary>
        /// <remarks>
        /// Maximum length: 1,024
        /// </remarks>
        /// <value>The name and process ID of the client.</value>
        [JsonPropertyName("source")]
        public string Source { get; set; }

        /// <summary>
        /// Request duration in format <c>DD.HH:MM:SS.MMMMMM</c>. Must be positive and less than 1,000 days.
        /// </summary>
        [JsonPropertyName("duration")]
        public string Duration { get; set; }

        /// <summary>
        /// Result of a request execution.
        /// </summary>
        /// <remarks>
        /// HTTP status codes for HTTP requests; HRESULT or exception type for other request types.
        /// </remarks>
        [JsonPropertyName("responseCode")]
        public string ResponseCode { get; set; }

        /// <summary>
        /// Indicates whether the request was successful or not.
        /// </summary>
        /// <remarks>
        /// Set this value to <c>false</c> if the operation was interrupted by exception or returned an error result code.
        /// </remarks>
        [JsonPropertyName("success")]
        public bool Success { get; set; } = true;        
    }
}
