namespace Rubberduck.SettingsProvider.Model.LanguageClient
{
    public record class RequireAddInHostSetting : RubberduckSetting<bool>
    {
        public static bool DefaultSettingValue { get; } = true;

        // TODO localize
        private static readonly string _description = "Whether the Rubberduck Editor is allowed to run without a VBIDE-connected Rubberduck add-in host; disables host-dependent features when false.";

        public RequireAddInHostSetting()
            : base(SettingDataType.BooleanSetting, nameof(RequireAddInHostSetting), _description, DefaultSettingValue)
        {
        }

        public RequireAddInHostSetting(bool value)
            : base(SettingDataType.BooleanSetting, nameof(RequireAddInHostSetting), _description, DefaultSettingValue, value)
        {
        }
    }
}
