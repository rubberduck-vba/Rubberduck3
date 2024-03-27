namespace Rubberduck.InternalApi.Settings.Model.TelemetryServer;

/// <summary>
/// Determines whether telemetry is enabled at all.
/// </summary>
public record class IsTelemetryEnabledSetting : BooleanRubberduckSetting
{
    public static bool DefaultSettingValue { get; } = false;

    public IsTelemetryEnabledSetting()
    {
        Value = DefaultValue = DefaultSettingValue;
    }
}
