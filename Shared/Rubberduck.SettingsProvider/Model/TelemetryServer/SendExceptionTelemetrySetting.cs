namespace Rubberduck.SettingsProvider.Model
{
    public record class SendExceptionTelemetrySetting : TypedRubberduckSetting<bool>
    {
        // TODO localize
        private static readonly string _description = "Determines whether exception telemetry data is transmitted.";

        public SendExceptionTelemetrySetting(bool defaultValue) : this(defaultValue, defaultValue) { }
        public SendExceptionTelemetrySetting(bool defaultValue, bool value)
            : base(nameof(SendExceptionTelemetrySetting), value, SettingDataType.BooleanSetting, defaultValue, readOnlyRecommended: defaultValue) { }
    }
}
