using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Rubberduck.RPC.Platform.Model.Telemetry
{
    public abstract class TelemetryEvent
    {
        protected TelemetryEvent(TelemetryEventName eventName)
        {
            EventName = eventName.ToString();
        }

        /// <summary>
        /// The name of the event.
        /// </summary>
        [JsonPropertyName("name")]
        public string EventName { get; set; }

        /// <summary>
        /// The context of the event.
        /// </summary>
        [JsonPropertyName("context")]
        public TelemetryContext Context { get; set; }

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
        public Dictionary<string, string> CustomProperties { get; set; }
    }
}
