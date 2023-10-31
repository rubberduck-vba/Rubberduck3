namespace Rubberduck.SettingsProvider.Model.LanguageClient
{
    /// <summary>
    /// Whether non-default workspaces are allowed to be defined using a UNC path (not recommended).
    /// </summary>
    public class EnableUncWorkspacesSetting : BooleanRubberduckSetting
    {
        public static bool DefaultSettingValue { get; } = false;

        public EnableUncWorkspacesSetting()
        {
            DefaultValue = DefaultSettingValue;
            Tags = SettingTags.Advanced | SettingTags.Experimental;
        }
    }
}
