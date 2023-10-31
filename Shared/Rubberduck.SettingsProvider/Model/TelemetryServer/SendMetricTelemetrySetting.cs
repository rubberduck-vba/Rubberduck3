namespace Rubberduck.SettingsProvider.Model.TelemetryServer
{
    /// <summary>
    /// Determines whether metric telemetry data is transmitted.
    /// </summary>
    public record class SendMetricTelemetrySetting : BooleanRubberduckSetting
    {
        public static bool DefaultSettingValue { get; } = true;

        public SendMetricTelemetrySetting()
        {
            DefaultValue = DefaultSettingValue;
        }
    }
}
