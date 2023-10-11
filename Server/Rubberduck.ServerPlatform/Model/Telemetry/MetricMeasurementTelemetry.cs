using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.ServerPlatform;
using Rubberduck.InternalApi.ServerPlatform.TelemetryServer;
using System.Text.Json.Serialization;

namespace Rubberduck.ServerPlatform.Model.Telemetry
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
    public record MetricMeasurementTelemetry : TelemetryEvent
    {
        public MetricMeasurementTelemetry(string name, double value, TelemetryEventParams request, TelemetryContext context) 
            : base(TelemetryEventName.Metric, request, context) 
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// The name of the command initiated with this dependency call.
        /// </summary>
        /// <remarks>
        /// Low cardinality value, e.g. stored procedure name, URL path template.
        /// </remarks>
        [JsonPropertyName("name")]
        public string Name { get; init; }

        /// <summary>
        /// Single value for measurement. Sum of individual measurements for an aggregate.
        /// </summary>
        [JsonPropertyName("value")]
        public double Value { get; init; }

        /// <summary>
        /// Metric weight of the aggregated metric.
        /// </summary>
        /// <remarks>
        /// Should not be set for a measurement.
        /// </remarks>
        [JsonPropertyName("count")]
        public int? Count { get; init; }

        /// <summary>
        /// Minimum value of the aggregated metric.
        /// </summary>
        /// <remarks>
        /// Should not be set for a measurement.
        /// </remarks>
        [JsonPropertyName("min")]
        public double? Minimum { get; init; }

        /// <summary>
        /// Maximum value of the aggregated metric.
        /// </summary>
        /// <remarks>
        /// Should not be set for a measurement.
        /// </remarks>
        [JsonPropertyName("max")]
        public double? Maximum { get; init; }

        /// <summary>
        /// The standard deviation of the aggregated metric.
        /// </summary>
        /// <remarks>
        /// Should not be set for a measurement.
        /// </remarks>
        [JsonPropertyName("stdev")]
        public double? StandardDeviation { get; init; }

        /// <summary>
        /// The average of the aggregated metric.
        /// </summary>
        /// <remarks>
        /// Should not be set for a measurement.
        /// </remarks>
        [JsonPropertyName("avg")]
        public double? Average { get; init; }
    }
}
