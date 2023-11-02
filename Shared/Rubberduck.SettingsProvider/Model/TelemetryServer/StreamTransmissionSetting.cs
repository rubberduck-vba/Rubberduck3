namespace Rubberduck.SettingsProvider.Model.TelemetryServer
{
    /// <summary>
    /// Determines whether telemetry data is transmitted automatically in periodic batches.
    /// </summary>
    public record class StreamTransmissionSetting : BooleanRubberduckSetting
    {
        public static bool DefaultSettingValue { get; } = false;

        public StreamTransmissionSetting()
        {
            DefaultValue = DefaultSettingValue;
        }
    }
}
