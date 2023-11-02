using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.ServerPlatform.TelemetryServer;
using System;
using System.Text.Json.Serialization;

namespace Rubberduck.ServerPlatform.Model.Telemetry
{
    /// <summary>
    /// <strong>RequestTelemetry</strong> represents the logical sequence of execution triggered by an external request to your application.
    /// Every request execution is identified by unique ID and URL containing all the execution parameters.
    /// RequestsTelemetry items may be regrouped by logical name and define the source of this request.
    /// Code execution may fail or succeed, and has a certain duration.
    /// </summary>
    public record RequestTelemetry : TelemetryEvent
    {
        public RequestTelemetry(string name, string id, Uri uri, string source, TimeSpan duration, string responseCode, bool success, TelemetryEventParams request, TelemetryContext context) 
            : base(TelemetryEventName.Request, request, context)
        {
            Name = name;
            Id = id;
            Url = uri.ToString();
            Source = source;
            Duration = duration.ToString();
            ResponseCode = responseCode;
            Success = success;
        }

        /// <summary>
        /// Name (URI) of the request, represents the code path taken to process the request.
        /// </summary>
        /// <remarks>
        /// Maximum value length: 1,024
        /// </remarks>
        [JsonPropertyName("name")]
        public string Name { get; init; }

        /// <summary>
        /// Identifies an instance of a request call; correlates between request and other telemetry items.
        /// </summary>
        /// <remarks>
        /// Value should be globally unique. Maximum length: 128
        /// </remarks>
        [JsonPropertyName("id")]
        public string Id { get; init; }

        /// <summary>
        /// Request URL with all query string parameters.
        /// </summary>
        /// <remarks>
        /// Maximum lenght: 2,048
        /// </remarks>
        [JsonPropertyName("url")]
        public string Url { get; init; }

        /// <summary>
        /// Source of the request.
        /// </summary>
        /// <remarks>
        /// Maximum length: 1,024
        /// </remarks>
        /// <value>The name and process ID of the client.</value>
        [JsonPropertyName("source")]
        public string Source { get; init; }

        /// <summary>
        /// Request duration in format <c>DD.HH:MM:SS.MMMMMM</c>. Must be positive and less than 1,000 days.
        /// </summary>
        [JsonPropertyName("duration")]
        public string Duration { get; init; }

        /// <summary>
        /// Result of a request execution.
        /// </summary>
        /// <remarks>
        /// HTTP status codes for HTTP requests; HRESULT or exception type for other request types.
        /// </remarks>
        [JsonPropertyName("responseCode")]
        public string ResponseCode { get; init; }

        /// <summary>
        /// Indicates whether the request was successful or not.
        /// </summary>
        /// <remarks>
        /// Set this value to <c>false</c> if the operation was interrupted by exception or returned an error result code.
        /// </remarks>
        [JsonPropertyName("success")]
        public bool Success { get; init; }
    }
}
