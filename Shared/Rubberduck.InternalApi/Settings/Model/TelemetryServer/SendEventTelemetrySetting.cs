namespace Rubberduck.InternalApi.Settings.Model.TelemetryServer;

/// <summary>
/// Determines whether event telemetry data is transmitted.
/// </summary>
public record class SendEventTelemetrySetting : BooleanRubberduckSetting
{
    public static bool DefaultSettingValue { get; } = false;

    public SendEventTelemetrySetting()
    {
        Value = DefaultValue = DefaultSettingValue;
    }
}
