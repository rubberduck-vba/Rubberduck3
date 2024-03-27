namespace Rubberduck.InternalApi.Settings.Model.TelemetryServer;

/// <summary>
/// Determines whether exception telemetry data is transmitted.
/// </summary>
public record class SendExceptionTelemetrySetting : BooleanRubberduckSetting
{
    public static bool DefaultSettingValue { get; } = true;

    public SendExceptionTelemetrySetting()
    {
        Value = DefaultValue = DefaultSettingValue;
    }
}
