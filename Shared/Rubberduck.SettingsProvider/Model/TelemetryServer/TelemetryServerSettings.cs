using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.ServerPlatform.TelemetryServer;
using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model.ServerStartup;
using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace Rubberduck.SettingsProvider.Model.TelemetryServer
{
    /// <summary>
    /// Configures telemetry server options.
    /// </summary>
    public record class TelemetryServerSettings : TypedSettingGroup, IDefaultSettingsProvider<TelemetryServerSettings>
    {
        private static readonly RubberduckSetting[] DefaultSettings =
            new RubberduckSetting[]
            {
                new TelemetryServerStartupSettings { Value = TelemetryServerStartupSettings.DefaultSettings },
                new TraceLevelSetting { Value = TraceLevelSetting.DefaultSettingValue },
                new IsTelemetryEnabledSetting { Value = IsTelemetryEnabledSetting.DefaultSettingValue },
                new StreamTransmissionSetting { Value = StreamTransmissionSetting.DefaultSettingValue },
                new TelemetryEventQueueSizeSetting { Value = TelemetryEventQueueSizeSetting.DefaultSettingValue },
                new SendEventTelemetrySetting { Value = SendEventTelemetrySetting.DefaultSettingValue },
                new SendExceptionTelemetrySetting { Value = SendExceptionTelemetrySetting.DefaultSettingValue },
                new SendMetricTelemetrySetting { Value = SendMetricTelemetrySetting.DefaultSettingValue },
                new SendTraceTelemetrySetting { Value = SendTraceTelemetrySetting.DefaultSettingValue },
                new ExceptionTelemetrySettings { Value = ExceptionTelemetrySettings.DefaultSettings },
                new TraceTelemetrySettings { Value = TraceTelemetrySettings.DefaultSettings },
                new EventTelemetrySettings { Value = EventTelemetrySettings.DefaultSettings },
                new MetricTelemetrySettings { Value = MetricTelemetrySettings.DefaultSettings },
            };

        public TelemetryServerSettings()
        {
            DefaultValue = DefaultSettings;
        }

        [JsonIgnore]
        public bool IsEnabled => GetSetting<IsTelemetryEnabledSetting>().TypedValue;
        [JsonIgnore]
        public MessageTraceLevel TraceLevel => GetSetting<TraceLevelSetting>().TypedValue;
        [JsonIgnore]
        public bool StreamTransmission => GetSetting<StreamTransmissionSetting>().TypedValue;
        [JsonIgnore]
        public int QueueSize => (int)GetSetting<TelemetryEventQueueSizeSetting>().TypedValue;
        [JsonIgnore]
        public bool SendEventTelemetry => GetSetting<SendEventTelemetrySetting>().TypedValue;
        [JsonIgnore]
        public bool SendExceptionTelemetry => GetSetting<SendExceptionTelemetrySetting>().TypedValue;
        [JsonIgnore]
        public bool SendMetricTelemetry => GetSetting<SendMetricTelemetrySetting>().TypedValue;
        [JsonIgnore]
        public bool SendTraceTelemetry => GetSetting<SendTraceTelemetrySetting>().TypedValue;
        [JsonIgnore]
        public TelemetryServerStartupSettings StartupSettings => GetSetting<TelemetryServerStartupSettings>();
        [JsonIgnore]
        public EventTelemetrySettings EventTelemetrySettings => GetSetting<EventTelemetrySettings>();
        [JsonIgnore]
        public ExceptionTelemetrySettings ExceptionTelemetrySettings => GetSetting<ExceptionTelemetrySettings>();
        [JsonIgnore]
        public MetricTelemetrySettings MetricTelemetrySettings => GetSetting<MetricTelemetrySettings>();
        [JsonIgnore]
        public TraceTelemetrySettings TraceTelemetrySettings => GetSetting<TraceTelemetrySettings>();

        public static TelemetryServerSettings Default { get; } = new TelemetryServerSettings { Value = DefaultSettings };
        TelemetryServerSettings IDefaultSettingsProvider<TelemetryServerSettings>.Default => Default;
    }

    public abstract record class TelemetrySettingGroup<TKey> : EnumSettingGroup<TKey>
        where TKey : struct, Enum
    {
        public bool IsEnabled(TKey key) => ((TelemetrySetting)GetSetting(key)).TypedValue;
    }

    public record class EventTelemetrySettings : TelemetrySettingGroup<EventTelemetryName>
    {
        public static TelemetrySetting[] DefaultSettings { get; } =
            Enum.GetValues<EventTelemetryName>().Select(e => new TelemetrySetting { Key = $"{nameof(EventTelemetrySettings)}.{e}", Value = false, DefaultValue = false }).ToArray();

        public EventTelemetrySettings()
        {
            SettingDataType = SettingDataType.EnumSettingGroup;
            DefaultValue = DefaultSettings;
        }
    }

    public record class ExceptionTelemetrySettings : TelemetrySettingGroup<LogLevel>
    {
        public static TelemetrySetting[] DefaultSettings { get; } =
            Enum.GetValues<LogLevel>().Select(e => new TelemetrySetting { Key = $"{nameof(ExceptionTelemetrySettings)}.{e}", Value = true, DefaultValue = true }).ToArray();

        public ExceptionTelemetrySettings()
        {
            SettingDataType = SettingDataType.EnumSettingGroup;
            DefaultValue = DefaultSettings;
        }
    }

    public record class MetricTelemetrySettings : TelemetrySettingGroup<MetricTelemetryName>
    {
        public static TelemetrySetting[] DefaultSettings { get; } =
            Enum.GetValues<MetricTelemetryName>().Select(e => new TelemetrySetting { Key = $"{nameof(MetricTelemetrySettings)}.{e}", Value = false, DefaultValue = false }).ToArray();

        public MetricTelemetrySettings()
        {
            SettingDataType = SettingDataType.EnumSettingGroup;
            DefaultValue = DefaultSettings;
        }
    }

    public record class TraceTelemetrySettings : TelemetrySettingGroup<LogLevel>
    {
        public static TelemetrySetting[] DefaultSettings { get; } =
            Enum.GetValues<LogLevel>().Select(e => new TelemetrySetting { Key = $"{nameof(TraceTelemetrySettings)}.{e}", Value = e >= LogLevel.Warning, DefaultValue = e >= LogLevel.Warning }).ToArray();

        public TraceTelemetrySettings()
        {
            SettingDataType = SettingDataType.EnumSettingGroup;
            DefaultValue = DefaultSettings;
        }
    }

    /// <summary>
    /// The base class for enabling/disabling individual granular telemetry events.
    /// </summary>
    public record class TelemetrySetting : BooleanRubberduckSetting
    {
        public TelemetrySetting()
        {
        }
    }
}
