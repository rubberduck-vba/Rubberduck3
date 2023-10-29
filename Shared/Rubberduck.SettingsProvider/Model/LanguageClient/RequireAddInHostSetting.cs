namespace Rubberduck.SettingsProvider.Model.LanguageClient
{
    public record class RequireAddInHostSetting : TypedRubberduckSetting<bool>
    {
        // TODO localize
        private static readonly string _description = "Whether the Rubberduck Editor is allowed to run without a VBIDE-connected Rubberduck add-in host; disables host-dependent features when false.";
        public static bool DefaultSettingValue { get; } = true;

        public RequireAddInHostSetting()
            : base(nameof(RequireAddInHostSetting), DefaultSettingValue, SettingDataType.BooleanSetting, DefaultSettingValue) { }

        public RequireAddInHostSetting(bool value)
            : base(nameof(RequireAddInHostSetting), value, SettingDataType.BooleanSetting, DefaultSettingValue) { }
    }
}
