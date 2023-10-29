using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.ServerPlatform.TelemetryServer;
using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model.ServerStartup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Rubberduck.SettingsProvider.Model
{
    public record class TelemetryServerSettings : TypedSettingGroup, IDefaultSettingsProvider<TelemetryServerSettings>
    {
        // TODO localize
        private static readonly string _description = "Configures telemetry server options.";
        private static readonly RubberduckSetting[] DefaultSettings = 
            new RubberduckSetting[]
            {
                new TelemetryServerStartupSettings(),
                new TraceLevelSetting(nameof(TraceLevelSetting), MessageTraceLevel.Verbose),
                new IsTelemetryEnabledSetting(true),
                new StreamTransmissionSetting(false),
                new TelemetryEventQueueSizeSetting(1000),
                new SendEventTelemetrySetting(true),
                new SendExceptionTelemetrySetting(true),
                new SendMetricTelemetrySetting(true),
                new SendTraceTelemetrySetting(false),
                new EventTelemetrySettings(),
                new ExceptionTelemetrySettings(),
                new MetricTelemetrySettings(),
                new TraceTelemetrySettings(),
            };

        public TelemetryServerSettings() 
            : base(nameof(TelemetryServerSettings), DefaultSettings, DefaultSettings) { }

        public TelemetryServerSettings(TelemetryServerSettings original, IEnumerable<RubberduckSetting>? settings)
            : base(original) 
        {
            Value = settings?.ToArray() ?? DefaultSettings;
        }

        public TelemetryServerSettings(params RubberduckSetting[] settings)
            : base(nameof(TelemetryServerSettings), settings, DefaultSettings) { }

        public TelemetryServerSettings(IEnumerable<RubberduckSetting> settings)
            : base(nameof(TelemetryServerSettings), settings, DefaultSettings) { }


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

        public static TelemetryServerSettings Default { get; } = new(DefaultSettings);
        TelemetryServerSettings IDefaultSettingsProvider<TelemetryServerSettings>.Default => Default;
    }

    public abstract record class TelemetrySettingGroup<TKey> : EnumSettingGroup<TKey>
        where TKey : struct, Enum
    {
        public TelemetrySettingGroup(string name, IEnumerable<TelemetrySetting> defaults)
            : base(name, defaults, defaults) { }
        public TelemetrySettingGroup(string name, IEnumerable<TelemetrySetting> settings, IEnumerable<TelemetrySetting> defaults)
            : base(name, settings, defaults) { }

        public bool IsEnabled(TKey key) => ((TelemetrySetting)GetSetting(key)).TypedValue;
    }

    public record class EventTelemetrySettings : TelemetrySettingGroup<EventTelemetryName>
    {
        private static TelemetrySetting[] DefaultSettings =
            Enum.GetValues<EventTelemetryName>().Select(e => new EventTelemetrySetting(e, true)).ToArray();

        public EventTelemetrySettings()
            : base(nameof(EventTelemetrySettings), DefaultSettings, DefaultSettings) { }

        public EventTelemetrySettings(IEnumerable<TelemetrySetting> settings) 
            : base(nameof(EventTelemetrySettings), settings, DefaultSettings) { }
    }

    public record class ExceptionTelemetrySettings : TelemetrySettingGroup<LogLevel>
    {
        private static TelemetrySetting[] DefaultSettings =
            Enum.GetValues<LogLevel>().Select(e => new ExceptionTelemetrySetting(e, true)).ToArray();

        public ExceptionTelemetrySettings()
            : base(nameof(ExceptionTelemetrySettings), DefaultSettings, DefaultSettings) { }
        public ExceptionTelemetrySettings(IEnumerable<TelemetrySetting> settings)
            : base(nameof(ExceptionTelemetrySettings), settings, DefaultSettings) { }
    }

    public record class MetricTelemetrySettings : TelemetrySettingGroup<MetricTelemetryName>
    {
        private static TelemetrySetting[] DefaultSettings =
            Enum.GetValues<MetricTelemetryName>().Select(e => new MetricTelemetrySetting(e, true)).ToArray();

        public MetricTelemetrySettings()
            : base(nameof(MetricTelemetrySettings), DefaultSettings, DefaultSettings) { }
        public MetricTelemetrySettings(IEnumerable<TelemetrySetting> settings)
            : base(nameof(MetricTelemetrySettings), settings, DefaultSettings) { }
    }

    public record class TraceTelemetrySettings : TelemetrySettingGroup<LogLevel>
    {
        private static TelemetrySetting[] DefaultSettings =
            Enum.GetValues<LogLevel>().Select(e => new TraceTelemetrySetting(e, true)).ToArray();

        public TraceTelemetrySettings()
            : base(nameof(TraceTelemetrySettings), DefaultSettings, DefaultSettings) { }
        public TraceTelemetrySettings(IEnumerable<TelemetrySetting> settings)
            : base(nameof(TraceTelemetrySettings), settings, DefaultSettings) { }
    }

    public abstract record class TelemetrySetting : TypedRubberduckSetting<bool>
    {
        protected TelemetrySetting(string name, bool defaultValue, bool readonlyRecommended = false)
            : base(name, defaultValue, SettingDataType.BooleanSetting, defaultValue, readonlyRecommended) { }
    }

    public record class EventTelemetrySetting : TelemetrySetting
    {
        public EventTelemetrySetting(EventTelemetryName name, bool defaultValue, bool readonlyRecommended = false)
            : base(name.ToString(), defaultValue, readonlyRecommended) { }
    }

    public record class ExceptionTelemetrySetting : TelemetrySetting
    {
        public ExceptionTelemetrySetting(LogLevel name, bool defaultValue, bool readonlyRecommended = true)
            : base(name.ToString(), defaultValue, readonlyRecommended) { }
    }

    public record class MetricTelemetrySetting : TelemetrySetting
    {
        public MetricTelemetrySetting(MetricTelemetryName name, bool defaultValue, bool readonlyRecommended = false)
            : base(name.ToString(), defaultValue, readonlyRecommended) { }
    }

    public record class TraceTelemetrySetting : TelemetrySetting
    {
        public TraceTelemetrySetting(LogLevel name, bool defaultValue, bool readonlyRecommended = false)
            : base(name.ToString(), defaultValue, readonlyRecommended) { }
    }
}
