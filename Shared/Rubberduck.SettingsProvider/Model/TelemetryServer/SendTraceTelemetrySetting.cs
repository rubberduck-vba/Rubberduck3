namespace Rubberduck.SettingsProvider.Model
{
    public record class SendTraceTelemetrySetting : RubberduckSetting<bool>
    {
        // TODO localize
        private static readonly string _description = "Determines whether trace telemetry data is transmitted.";

        public SendTraceTelemetrySetting(bool defaultValue) : this(defaultValue, defaultValue) { }
        public SendTraceTelemetrySetting(bool defaultValue, bool value)
            : base(SettingDataType.BooleanSetting, nameof(SendTraceTelemetrySetting), _description, defaultValue, value)
        {
        }
    }
}
