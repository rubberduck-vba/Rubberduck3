namespace Rubberduck.SettingsProvider.Model.TelemetryServer
{
    /// <summary>
    /// Determines whether trace telemetry data is transmitted.
    /// </summary>
    public class SendTraceTelemetrySetting : BooleanRubberduckSetting
    {
        public static bool DefaultSettingValue { get; } = false;

        public SendTraceTelemetrySetting()
        {
            DefaultValue = DefaultSettingValue;
        }
    }
}
