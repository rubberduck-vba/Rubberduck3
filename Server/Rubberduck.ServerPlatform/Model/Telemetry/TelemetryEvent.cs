using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.ServerPlatform.TelemetryServer;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rubberduck.ServerPlatform.Model.Telemetry
{
    public abstract record TelemetryEvent : TelemetryEventPayload
    {
        protected TelemetryEvent(TelemetryEventName eventName, TelemetryEventParams request, TelemetryContext context)
            : base(request)
        {
            EventName = eventName.ToString();
            Context = context;
            CustomProperties = new Dictionary<string, string>();
        }

        /// <summary>
        /// The name of the event.
        /// </summary>
        [JsonPropertyName("name")]
        public string EventName { get; init; }

        /// <summary>
        /// The context of the event.
        /// </summary>
        [JsonPropertyName("context")]
        public TelemetryContext Context { get; init; }

        /// <summary>
        /// Name-value collection of custom properties, used to extend standard telemetry with the custom dimensions.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item>Maximum key length: 150</item>
        /// <item>Maximum value length: 8,192</item>
        /// </list>
        /// </remarks>
        [JsonPropertyName("customProperties")]
        public Dictionary<string, string> CustomProperties { get; init; }
    }
}
