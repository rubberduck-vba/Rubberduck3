using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Platform.Model.Telemetry
{
    /// <summary>
    /// <strong>PageViewTelemetry</strong> is logged when the user opens a new <em>page</em> (/tab/screen) of a monitored application.
    /// The <em>page</em> in this context is a logical unit that is defined by the developer to be an application tab or screen.
    /// </summary>
    public class PageViewTelemetry : TelemetryEvent
    {
        public PageViewTelemetry() : base(TelemetryEventName.PageView) { }

        /// <summary>
        /// The name of the page or application resource that was viewed.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
