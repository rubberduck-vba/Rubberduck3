using System.Text.Json.Serialization;

namespace Rubberduck.ServerPlatform.Model.Telemetry
{
    /// <summary>
    /// <strong>ContextTelemetry</strong> collects additional context information and is included in all telemetry events.
    /// </summary>
    public record TelemetryContext
    {
        /// <summary>
        /// Information in the application context fields is always about the application that's sending the telemetry.
        /// The application version is used to analyze trend changes in the application behavior and its correlation to the deployments.
        /// </summary>
        /// <remarks>Maximum length: 1,024</remarks>
        [JsonPropertyName("applicationVersion")]
        public string ApplicationVersion { get; init; }

        /// <summary>
        /// The unique identifier of the <strong>root operation</strong>. Allows grouping telemetry across multiple components.
        /// </summary>
        [JsonPropertyName("operationId")]
        public string OperationId { get; init; }

        /// <summary>
        /// The unique identifier of the telemetry item's <em>immediate parent</em>.
        /// </summary>
        [JsonPropertyName("parentOperationId")]
        public string ParentOperationId { get; init; }

        /// <summary>
        /// The name (group) of the operation, created either by a <c>RequestTelemetry</c> or a <c>PageViewTelemetry</c> item: all other telemetry items
        /// set this field to the value of the <em>containing</em> request or page view.
        /// </summary>
        /// <remarks>
        /// Used for finding all telemetry items for a group of operations.
        /// </remarks>
        [JsonPropertyName("operationName")]
        public string OperationName { get; init; }

        /// <summary>
        /// Represents an instance of the user's interaction with the application. Information in the session context fields is always about the <em>user</em>.
        /// When telemetry is sent from a service, the session context is about the user that initiated the operation in the service.
        /// </summary>
        /// <remarks>
        /// Maximum length: 64
        /// </remarks>
        [JsonPropertyName("sessionId")]
        public string SessionId { get; init; }

        /// <summary>
        /// Represents the user of the application. <strong>DO NOT use this field to store a user name or identifier.</strong>
        /// </summary>
        /// <remarks>
        /// User IDs can be cross referenced with session IDs to provide unique telemetry dimensions and establish user activity over a session duration.
        /// </remarks>
        [JsonPropertyName("anonymousUserId")]
        public string AnonymousUserId { get; init; }

        /// <summary>
        /// Represents an authenticated user of the application.
        /// </summary>
        /// <remarks>
        /// User IDs can be cross referenced with session IDs to provide unique telemetry dimensions and establish user activity over a session duration.
        /// </remarks>
        [JsonPropertyName("authenticatedUserId")]
        public string AuthenticatedUserId { get; init; }
    }
}
