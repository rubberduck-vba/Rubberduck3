namespace Rubberduck.SettingsProvider.Model
{
    public record class TelemetryEventQueueSizeSetting : TypedRubberduckSetting<double>
    {
        // TODO localize
        private static readonly string _description = "The maximum number of telemetry events transmission payload.";

        public TelemetryEventQueueSizeSetting(double defaultValue) : this(defaultValue, defaultValue) { }
        public TelemetryEventQueueSizeSetting(double defaultValue, double value)
            : base(nameof(TelemetryEventQueueSizeSetting), value, SettingDataType.NumericSetting, defaultValue) { }
    }
}
