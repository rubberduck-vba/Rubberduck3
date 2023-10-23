using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.ServerPlatform.TelemetryServer;
using Rubberduck.InternalApi.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using static Rubberduck.SettingsProvider.Model.TelemetryServerSettingsGroup;

namespace Rubberduck.SettingsProvider.Model
{
    public interface ITelemetryServerSettings
    {
         bool IsEnabled { get; }
         bool StreamTransmission { get; }
         int QueueSize { get; }

         bool SendEventTelemetry { get; }
         bool SendExceptionTelemetry { get; }
         bool SendMetricTelemetry { get; }
         bool SendTraceTelemetry { get; }

        IReadOnlyDictionary<string, EventTelemetrySetting> EventTelemetryConfig { get; }
        IReadOnlyDictionary<string, ExceptionTelemetrySetting> ExceptionTelemetryConfig { get; }
        IReadOnlyDictionary<string, MetricTelemetrySetting> MetricTelemetryConfig { get; }
        IReadOnlyDictionary<string, TraceTelemetrySetting> TraceTelemetryConfig { get; }
    }

    public record class TelemetryServerSettingsGroup : SettingGroup, IDefaultSettingsProvider<TelemetryServerSettingsGroup>, ITelemetryServerSettings
    {
        // TODO localize
        private static readonly string _description = "Configures telemetry server options.";

        public TelemetryServerSettingsGroup()
            : base(nameof(TelemetryServerSettingsGroup), _description)
        {
            EventTelemetryConfig = Enum.GetValues<EventTelemetryName>().Select(e => new EventTelemetrySetting { Id = e, IsEnabled = true }).ToDictionary(e => e.Key);
            ExceptionTelemetryConfig = Enum.GetValues<LogLevel>().Select(e => new ExceptionTelemetrySetting { Id = e, IsEnabled = true }).ToDictionary(e => e.Key);
            MetricTelemetryConfig = Enum.GetValues<MetricTelemetryName>().Select(e => new MetricTelemetrySetting { Id = e, IsEnabled = true }).ToDictionary(e => e.Key);
            TraceTelemetryConfig = Enum.GetValues<LogLevel>().Select(e => new TraceTelemetrySetting { Id = e, IsEnabled = true, Verbose = true }).ToDictionary(e => e.Key);
        }

        public bool IsEnabled => bool.Parse(Values[nameof(IsTelemetryEnabledSetting)]);
        public bool StreamTransmission => bool.Parse(Values[nameof(StreamTransmissionSetting)]);
        public int QueueSize => int.Parse(Values[nameof(TelemetryEventQueueSizeSetting)]);

        public bool SendEventTelemetry => bool.Parse(Values[nameof(SendEventTelemetrySetting)]);
        public bool SendExceptionTelemetry => bool.Parse(Values[nameof(SendExceptionTelemetrySetting)]);
        public bool SendMetricTelemetry => bool.Parse(Values[nameof(SendMetricTelemetrySetting)]);
        public bool SendTraceTelemetry => bool.Parse(Values[nameof(SendTraceTelemetrySetting)]);

        public IReadOnlyDictionary<string, EventTelemetrySetting> EventTelemetryConfig { get; init; }
        public IReadOnlyDictionary<string, ExceptionTelemetrySetting> ExceptionTelemetryConfig { get; init; }
        public IReadOnlyDictionary<string, MetricTelemetrySetting> MetricTelemetryConfig { get; init; }
        public IReadOnlyDictionary<string, TraceTelemetrySetting> TraceTelemetryConfig { get; init; }

        protected override IEnumerable<RubberduckSetting> Settings { get; init; } = new RubberduckSetting[]
        {
            new IsTelemetryEnabledSetting(true),
            new StreamTransmissionSetting(false),
            new TelemetryEventQueueSizeSetting(1000),
            new SendEventTelemetrySetting(true),
            new SendExceptionTelemetrySetting(true),
            new SendMetricTelemetrySetting(true),
            new SendTraceTelemetrySetting(false),
        };

        public static TelemetryServerSettingsGroup Default { get; } = new();
        TelemetryServerSettingsGroup IDefaultSettingsProvider<TelemetryServerSettingsGroup>.Default => Default;

        public abstract record class TelemetrySetting
        {
            public abstract string Key { get; }
            public bool IsEnabled { get; init; }
        }

        public record class EventTelemetrySetting : TelemetrySetting
        {
            public override string Key => Id.ToString();
            public EventTelemetryName Id { get; init; }
        }

        public record class ExceptionTelemetrySetting : TelemetrySetting
        {
            public override string Key => Id.ToString();
            public LogLevel Id { get; init; }
        }

        public record class MetricTelemetrySetting : TelemetrySetting
        {
            public override string Key => Id.ToString();
            public MetricTelemetryName Id { get; init; }
        }

        public record class TraceTelemetrySetting : TelemetrySetting
        {
            public override string Key => Id.ToString();
            public LogLevel Id { get; init; }
            public bool Verbose { get; init; }
        }
    }
}
