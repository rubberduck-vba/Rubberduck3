namespace Rubberduck.SettingsProvider.Model
{
    public record class IsUpdateServerEnabledSetting : RubberduckSetting<bool>
    {
        // TODO localize
        private static readonly string _description = "Determines whether the update server is enabled at all.";

        public IsUpdateServerEnabledSetting(bool defaultValue) : this(defaultValue, defaultValue) { }
        public IsUpdateServerEnabledSetting(bool defaultValue, bool value)
            : base(nameof(IsUpdateServerEnabledSetting), value, SettingDataType.BooleanSetting, defaultValue) { }
    }
}
