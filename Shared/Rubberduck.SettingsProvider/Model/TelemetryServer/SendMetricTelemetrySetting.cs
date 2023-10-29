namespace Rubberduck.SettingsProvider.Model
{
    public record class SendMetricTelemetrySetting : TypedRubberduckSetting<bool>
    {
        // TODO localize
        private static readonly string _description = "Determines whether metric telemetry data is transmitted.";

        public SendMetricTelemetrySetting(bool defaultValue) : this(defaultValue, defaultValue) { }
        public SendMetricTelemetrySetting(bool defaultValue, bool value)
            : base(nameof(SendMetricTelemetrySetting), value, SettingDataType.BooleanSetting, defaultValue) { }
    }
}
