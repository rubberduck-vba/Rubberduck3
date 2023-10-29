namespace Rubberduck.SettingsProvider.Model.LanguageClient
{
    public record class RequireSavedHostSetting : TypedRubberduckSetting<bool>
    {
        public static bool DefaultSettingValue { get; } = true;

        // TODO localize
        private static readonly string _description = "Whether projects are required to be created from a saved host document.";

        public RequireSavedHostSetting()
            : base(nameof(RequireSavedHostSetting), DefaultSettingValue, SettingDataType.BooleanSetting, DefaultSettingValue) { }

        public RequireSavedHostSetting(bool value)
            : base(nameof(RequireSavedHostSetting), value, SettingDataType.BooleanSetting, DefaultSettingValue) { }
    }
}
