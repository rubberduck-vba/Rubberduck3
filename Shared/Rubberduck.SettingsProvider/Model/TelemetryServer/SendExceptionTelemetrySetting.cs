namespace Rubberduck.SettingsProvider.Model
{
    public record class SendExceptionTelemetrySetting : RubberduckSetting<bool>
    {
        // TODO localize
        private static readonly string _description = "Determines whether exception telemetry data is transmitted.";

        public SendExceptionTelemetrySetting(bool defaultValue) : this(defaultValue, defaultValue) { }
        public SendExceptionTelemetrySetting(bool defaultValue, bool value)
            : base(SettingDataType.BooleanSetting, nameof(SendExceptionTelemetrySetting), _description, defaultValue, value, readOnlyRecommended: defaultValue)
        {
        }
    }
}
