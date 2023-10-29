namespace Rubberduck.SettingsProvider.Model.LanguageClient
{
    public record class RequireDefaultWorkspaceRootHostSetting : RubberduckSetting<bool>
    {
        public static bool DefaultSettingValue { get; } = true;

        // TODO localize
        private static readonly string _description = "Whether host documents are required to be saved in a folder under the default workspace root.";

        public RequireDefaultWorkspaceRootHostSetting()
            : base(nameof(RequireDefaultWorkspaceRootHostSetting), DefaultSettingValue, SettingDataType.BooleanSetting, DefaultSettingValue) { }

        public RequireDefaultWorkspaceRootHostSetting(bool value)
            : base(nameof(RequireDefaultWorkspaceRootHostSetting), value, SettingDataType.BooleanSetting, DefaultSettingValue) { }
    }
}
