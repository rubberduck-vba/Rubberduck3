namespace Rubberduck.SettingsProvider.Model.LanguageClient
{
    public record class EnableUncWorkspacesSetting : RubberduckSetting<bool>
    {
        public static bool DefaultSettingValue { get; } = false;

        // TODO localize
        private static readonly string _description = "Whether non-default workspaces are allowed to be defined using a UNC path (not recommended).";

        public EnableUncWorkspacesSetting()
            : base(SettingDataType.BooleanSetting, nameof(EnableUncWorkspacesSetting), _description, DefaultSettingValue)
        {
        }

        public EnableUncWorkspacesSetting(bool value)
            : base(SettingDataType.BooleanSetting, nameof(EnableUncWorkspacesSetting), _description, DefaultSettingValue, value)
        {
        }
    }
}
