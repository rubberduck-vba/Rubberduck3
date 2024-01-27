namespace Rubberduck.InternalApi.Settings.Model.TelemetryServer;

/// <summary>
/// Determines whether trace telemetry data is transmitted.
/// </summary>
public record class SendTraceTelemetrySetting : BooleanRubberduckSetting
{
    public static bool DefaultSettingValue { get; } = false;

    public SendTraceTelemetrySetting()
    {
        DefaultValue = DefaultSettingValue;
    }
}
