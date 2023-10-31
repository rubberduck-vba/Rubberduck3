namespace Rubberduck.SettingsProvider.Model.TelemetryServer
{
    /// <summary>
    /// Determines whether event telemetry data is transmitted.
    /// </summary>
    public class SendEventTelemetrySetting : BooleanRubberduckSetting
    {
        public static bool DefaultSettingValue { get; } = false;

        public SendEventTelemetrySetting()
        {
            DefaultValue = DefaultSettingValue;
        }
    }
}
