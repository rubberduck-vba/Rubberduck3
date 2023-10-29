namespace Rubberduck.SettingsProvider.Model
{
    public record class SendEventTelemetrySetting : TypedRubberduckSetting<bool>
    {
        // TODO localize
        private static readonly string _description = "Determines whether event telemetry data is transmitted.";

        public SendEventTelemetrySetting(bool defaultValue) : this(defaultValue, defaultValue) { }
        public SendEventTelemetrySetting(bool defaultValue, bool value)
            : base(nameof(SendEventTelemetrySetting), value, SettingDataType.BooleanSetting, defaultValue) { }
    }
}
