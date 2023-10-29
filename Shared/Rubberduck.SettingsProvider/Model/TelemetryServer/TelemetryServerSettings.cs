using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.ServerPlatform.TelemetryServer;
using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model.ServerStartup;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Rubberduck.SettingsProvider.Model
{
    public record class TelemetryServerSettings : SettingGroup, IDefaultSettingsProvider<TelemetryServerSettings>
    {
        // TODO localize
        private static readonly string _description = "Configures telemetry server options.";
        private static readonly IRubberduckSetting[] DefaultSettings = 
            new IRubberduckSetting[]
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

        public TelemetryServerSettings(TelemetryServerSettings original, IEnumerable<IRubberduckSetting>? settings)
            : base(original) 
        {
            Value = settings?.ToArray() ?? DefaultSettings;
        }

        public TelemetryServerSettings(params IRubberduckSetting[] settings)
            : base(nameof(TelemetryServerSettings), settings, DefaultSettings) { }

        public TelemetryServerSettings(IEnumerable<IRubberduckSetting> settings)
            : base(nameof(TelemetryServerSettings), settings, DefaultSettings) { }


        public bool IsEnabled => GetSetting<IsTelemetryEnabledSetting>().Value;
        public MessageTraceLevel TraceLevel => GetSetting<TraceLevelSetting>().Value;
        public bool StreamTransmission => GetSetting<StreamTransmissionSetting>().Value;
        public int QueueSize => (int)GetSetting<TelemetryEventQueueSizeSetting>().Value;
        public bool SendEventTelemetry => GetSetting<SendEventTelemetrySetting>().Value;
        public bool SendExceptionTelemetry => GetSetting<SendExceptionTelemetrySetting>().Value;
        public bool SendMetricTelemetry => GetSetting<SendMetricTelemetrySetting>().Value;
        public bool SendTraceTelemetry => GetSetting<SendTraceTelemetrySetting>().Value;
        public TelemetryServerStartupSettings StartupSettings => GetSetting<TelemetryServerStartupSettings>();
        public EventTelemetrySettings EventTelemetrySettings => GetSetting<EventTelemetrySettings>();
        public ExceptionTelemetrySettings ExceptionTelemetrySettings => GetSetting<ExceptionTelemetrySettings>();
        public MetricTelemetrySettings MetricTelemetrySettings => GetSetting<MetricTelemetrySettings>();
        public TraceTelemetrySettings TraceTelemetrySettings => GetSetting<TraceTelemetrySettings>();

        public static TelemetryServerSettings Default { get; } = new(DefaultSettings);
        TelemetryServerSettings IDefaultSettingsProvider<TelemetryServerSettings>.Default => Default;
    }

    public abstract record class TelemetrySettingGroup<TKey> : SettingGroup
    {
        public TelemetrySettingGroup(string name, IEnumerable<TelemetrySetting> defaults)
            : base(name, defaults, defaults) { }
        public TelemetrySettingGroup(string name, IEnumerable<TelemetrySetting> settings, IEnumerable<TelemetrySetting> defaults)
            : base(name, settings, defaults) { }

        public TelemetrySetting GetSetting(TKey key) => Value.OfType<TelemetrySetting>().Single(e => e.NameKey == key!.ToString());

        public bool IsEnabled(TKey key) => GetSetting(key).Value;
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

    public abstract record class TelemetrySetting : RubberduckSetting<bool>
    {
        protected TelemetrySetting(string name, bool defaultValue, bool readonlyRecommended = false)
            : base(name, defaultValue, SettingDataType.BooleanSetting, defaultValue, readonlyRecommended) { }
    }

    public record class EventTelemetrySetting : TelemetrySetting
    {
        public EventTelemetrySetting(EventTelemetryName name, bool defaultValue, bool readonlyRecommended = false)
            : base($"{nameof(EventTelemetrySetting)}.{name}", defaultValue, readonlyRecommended) { }
    }

    public record class ExceptionTelemetrySetting : TelemetrySetting
    {
        public ExceptionTelemetrySetting(LogLevel name, bool defaultValue, bool readonlyRecommended = true)
            : base($"{nameof(ExceptionTelemetrySetting)}.{name}", defaultValue, readonlyRecommended) { }
    }

    public record class MetricTelemetrySetting : TelemetrySetting
    {
        public MetricTelemetrySetting(MetricTelemetryName name, bool defaultValue, bool readonlyRecommended = false)
            : base($"{nameof(MetricTelemetrySetting)}.{name}", defaultValue, readonlyRecommended) { }
    }

    public record class TraceTelemetrySetting : TelemetrySetting
    {
        public TraceTelemetrySetting(LogLevel name, bool defaultValue, bool readonlyRecommended = false)
            : base($"{nameof(TraceTelemetrySetting)}.{name}", defaultValue, readonlyRecommended) { }
    }
}
