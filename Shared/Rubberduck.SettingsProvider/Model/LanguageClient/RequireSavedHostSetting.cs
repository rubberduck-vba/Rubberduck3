namespace Rubberduck.SettingsProvider.Model.LanguageClient
{
    public record class RequireSavedHostSetting : RubberduckSetting<bool>
    {
        public static bool DefaultSettingValue { get; } = true;

        // TODO localize
        private static readonly string _description = "Whether projects are required to be created from a saved host document.";

        public RequireSavedHostSetting()
            : base(SettingDataType.BooleanSetting, nameof(RequireSavedHostSetting), _description, DefaultSettingValue)
        {
        }

        public RequireSavedHostSetting(bool value)
            : base(SettingDataType.BooleanSetting, nameof(RequireSavedHostSetting), _description, DefaultSettingValue, value)
        {
        }
    }
}
