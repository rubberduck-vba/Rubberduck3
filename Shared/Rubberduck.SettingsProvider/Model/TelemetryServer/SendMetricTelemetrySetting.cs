namespace Rubberduck.SettingsProvider.Model
{
    public record class SendMetricTelemetrySetting : RubberduckSetting<bool>
    {
        // TODO localize
        private static readonly string _description = "Determines whether metric telemetry data is transmitted.";

        public SendMetricTelemetrySetting(bool defaultValue) : this(defaultValue, defaultValue) { }
        public SendMetricTelemetrySetting(bool defaultValue, bool value)
            : base(SettingDataType.BooleanSetting, nameof(SendMetricTelemetrySetting), _description, defaultValue, value)
        {
        }
    }
}
