using System.Text.Json.Serialization;

namespace Rubberduck.RPC.Platform.Model.Telemetry
{
    /// <summary>
    /// <strong>MetricTelemetry</strong> represent various aggregatable measures.
    /// <list type="bullet">
    /// <item><em>Single measurement</em> metrics consist of a name and a value.</item>
    /// <item><em>Pre-aggregated</em> metrics specifies standard deviation, minimum, and maximum value of the metric in the aggregation interval.</item>
    /// </list>
    /// </summary>
    /// <remarks>
    /// Pre-aggregated telemetry assumes that aggregation period was one minute.
    /// </remarks>
    public class MetricTelemetry : TelemetryEvent
    {
        public MetricTelemetry() : base(TelemetryEventName.Metric) { }

        /// <summary>
        /// The name of the command initiated with this dependency call.
        /// </summary>
        /// <remarks>
        /// Low cardinality value, e.g. stored procedure name, URL path template.
        /// </remarks>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Single value for measurement. Sum of individual measurements for an aggregate.
        /// </summary>
        [JsonPropertyName("value")]
        public double Value { get; set; }

        /// <summary>
        /// Metric weight of the aggregated metric.
        /// </summary>
        /// <remarks>
        /// Should not be set for a measurement.
        /// </remarks>
        [JsonPropertyName("count")]
        public int? Count { get; set; }

        /// <summary>
        /// Minimum value of the aggregated metric.
        /// </summary>
        /// <remarks>
        /// Should not be set for a measurement.
        /// </remarks>
        [JsonPropertyName("min")]
        public double? Minimum { get; set; }

        /// <summary>
        /// Maximum value of the aggregated metric.
        /// </summary>
        /// <remarks>
        /// Should not be set for a measurement.
        /// </remarks>
        [JsonPropertyName("max")]
        public double? Maximum { get; set; }

        /// <summary>
        /// The standard deviation of the aggregated metric.
        /// </summary>
        /// <remarks>
        /// Should not be set for a measurement.
        /// </remarks>
        [JsonPropertyName("stdev")]
        public double? StandardDeviation { get; set; }

        /// <summary>
        /// The average of the aggregated metric.
        /// </summary>
        /// <remarks>
        /// Should not be set for a measurement.
        /// </remarks>
        [JsonPropertyName("avg")]
        public double? Average { get; set; }
    }
}
