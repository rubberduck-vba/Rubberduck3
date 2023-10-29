namespace Rubberduck.SettingsProvider.Model
{
    public record class SendTraceTelemetrySetting : TypedRubberduckSetting<bool>
    {
        // TODO localize
        private static readonly string _description = "Determines whether trace telemetry data is transmitted.";

        public SendTraceTelemetrySetting(bool defaultValue) : this(defaultValue, defaultValue) { }
        public SendTraceTelemetrySetting(bool defaultValue, bool value)
            : base(nameof(SendTraceTelemetrySetting), value, SettingDataType.BooleanSetting, defaultValue) { }
    }
}
