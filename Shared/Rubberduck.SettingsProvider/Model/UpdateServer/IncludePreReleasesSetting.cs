namespace Rubberduck.SettingsProvider.Model
{
    public record class IncludePreReleasesSetting : TypedRubberduckSetting<bool>
    {
        // TODO localize
        private static readonly string _description = "Determines whether checking for a newer version includes or ignores pre-release builds.";

        public IncludePreReleasesSetting(bool defaultValue) : this(defaultValue, defaultValue) { }
        public IncludePreReleasesSetting(bool defaultValue, bool value)
            : base(nameof(IncludePreReleasesSetting), value, SettingDataType.BooleanSetting, defaultValue) { }
    }
}
