namespace Rubberduck.SettingsProvider.Model.TelemetryServer
{
    /// <summary>
    /// Determines whether telemetry is enabled at all.
    /// </summary>
    public class IsTelemetryEnabledSetting : BooleanRubberduckSetting
    {
        public static bool DefaultSettingValue { get; } = true;

        public IsTelemetryEnabledSetting()
        {
            DefaultValue = DefaultSettingValue;
        }
    }
}
