namespace Rubberduck.InternalApi.Settings.Model.TelemetryServer;

/// <summary>
/// Determines whether telemetry is enabled at all.
/// </summary>
public record class IsTelemetryEnabledSetting : BooleanRubberduckSetting
{
    public static bool DefaultSettingValue { get; } = true;

    public IsTelemetryEnabledSetting()
    {
        DefaultValue = DefaultSettingValue;
    }
}
