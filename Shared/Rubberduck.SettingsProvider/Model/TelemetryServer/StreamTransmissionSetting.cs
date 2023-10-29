namespace Rubberduck.SettingsProvider.Model
{
    public record class StreamTransmissionSetting : RubberduckSetting<bool>
    {
        // TODO localize
        private static readonly string _description = "Determines whether telemetry data is transmitted automatically in periodic batches.";

        public StreamTransmissionSetting(bool defaultValue) : this(defaultValue, defaultValue) { }
        public StreamTransmissionSetting(bool defaultValue, bool value)
            : base(nameof(StreamTransmissionSetting), value, SettingDataType.BooleanSetting, defaultValue) { }
    }
}
