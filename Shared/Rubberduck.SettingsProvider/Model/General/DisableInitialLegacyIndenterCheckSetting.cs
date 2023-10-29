namespace Rubberduck.SettingsProvider.Model
{
    public record class DisableInitialLegacyIndenterCheckSetting : RubberduckSetting<bool>
    {
        public static bool DefaultSettingValue { get; } = false;

        private static readonly string _description = "Allow Rubberduck to scan the registry for legacy Smart Indenter settings after a successful initialization and shutdown; this initial check normally gets automatically disabled otherwise.";

        public DisableInitialLegacyIndenterCheckSetting() : this(DefaultSettingValue) { }

        public DisableInitialLegacyIndenterCheckSetting(bool value)
            : base(nameof(DisableInitialLegacyIndenterCheckSetting), value, SettingDataType.BooleanSetting, DefaultSettingValue) { }
    }
}
