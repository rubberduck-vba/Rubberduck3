using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.ServerPlatform.TelemetryServer;
using Rubberduck.InternalApi.Settings;
using Rubberduck.InternalApi.Settings.Model.ServerStartup;
using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.Settings.Model.TelemetryServer;

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
    public bool IsEnabled => GetSetting<IsTelemetryEnabledSetting>()?.TypedValue ?? IsTelemetryEnabledSetting.DefaultSettingValue;
    [JsonIgnore]
    public MessageTraceLevel TraceLevel => GetSetting<TraceLevelSetting>()?.TypedValue ?? TraceLevelSetting.DefaultSettingValue;
    [JsonIgnore]
    public bool StreamTransmission => GetSetting<StreamTransmissionSetting>()?.TypedValue ?? StreamTransmissionSetting.DefaultSettingValue;
    [JsonIgnore]
    public int QueueSize => (int)(GetSetting<TelemetryEventQueueSizeSetting>()?.TypedValue ?? TelemetryEventQueueSizeSetting.DefaultSettingValue);
    [JsonIgnore]
    public bool SendEventTelemetry => GetSetting<SendEventTelemetrySetting>()?.TypedValue ?? SendEventTelemetrySetting.DefaultSettingValue;
    [JsonIgnore]
    public bool SendExceptionTelemetry => GetSetting<SendExceptionTelemetrySetting>()?.TypedValue ?? SendExceptionTelemetrySetting.DefaultSettingValue;
    [JsonIgnore]
    public bool SendMetricTelemetry => GetSetting<SendMetricTelemetrySetting>()?.TypedValue ?? SendMetricTelemetrySetting.DefaultSettingValue;
    [JsonIgnore]
    public bool SendTraceTelemetry => GetSetting<SendTraceTelemetrySetting>()?.TypedValue ?? SendTraceTelemetrySetting.DefaultSettingValue;
    [JsonIgnore]
    public TelemetryServerStartupSettings StartupSettings => GetSetting<TelemetryServerStartupSettings>() ?? TelemetryServerStartupSettings.Default;
    [JsonIgnore]
    public EventTelemetrySettings EventTelemetrySettings => GetSetting<EventTelemetrySettings>() ?? EventTelemetrySettings.Default;
    [JsonIgnore]
    public ExceptionTelemetrySettings ExceptionTelemetrySettings => GetSetting<ExceptionTelemetrySettings>() ?? ExceptionTelemetrySettings.Default;
    [JsonIgnore]
    public MetricTelemetrySettings MetricTelemetrySettings => GetSetting<MetricTelemetrySettings>() ?? MetricTelemetrySettings.Default;
    [JsonIgnore]
    public TraceTelemetrySettings TraceTelemetrySettings => GetSetting<TraceTelemetrySettings>() ?? TraceTelemetrySettings.Default;

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

    public static EventTelemetrySettings Default { get; } = new() { DefaultValue = DefaultSettings, Value = DefaultSettings };
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

    public static ExceptionTelemetrySettings Default { get; } = new() { DefaultValue = DefaultSettings, Value = DefaultSettings };
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

    public static MetricTelemetrySettings Default { get; } = new() { DefaultValue = DefaultSettings, Value = DefaultSettings };
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

    public static TraceTelemetrySettings Default { get; } = new() { DefaultValue = DefaultSettings, Value = DefaultSettings };
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
