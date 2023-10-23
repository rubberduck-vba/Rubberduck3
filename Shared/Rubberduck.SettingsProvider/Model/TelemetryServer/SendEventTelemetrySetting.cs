namespace Rubberduck.SettingsProvider.Model
{
    public record class SendEventTelemetrySetting : RubberduckSetting<bool>
    {
        // TODO localize
        private static readonly string _description = "Determines whether event telemetry data is transmitted.";

        public SendEventTelemetrySetting(bool defaultValue) : this(defaultValue, defaultValue) { }
        public SendEventTelemetrySetting(bool defaultValue, bool value)
            : base(SettingDataType.BooleanSetting, nameof(SendEventTelemetrySetting), _description, defaultValue, value)
        {
        }
    }
}
