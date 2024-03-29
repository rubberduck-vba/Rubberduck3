﻿namespace Rubberduck.InternalApi.Settings.Model.TelemetryServer;

/// <summary>
/// Determines whether metric telemetry data is transmitted.
/// </summary>
public record class SendMetricTelemetrySetting : BooleanRubberduckSetting
{
    public static bool DefaultSettingValue { get; } = true;

    public SendMetricTelemetrySetting()
    {
        Value = DefaultValue = DefaultSettingValue;
    }
}
