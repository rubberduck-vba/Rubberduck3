namespace Rubberduck.SettingsProvider.Model.LanguageClient
{
    /// <summary>
    /// Whether host documents are required to be saved in a folder under the default workspace root.
    /// </summary>
    public class RequireDefaultWorkspaceRootHostSetting : BooleanRubberduckSetting
    {
        public static bool DefaultSettingValue { get; } = true;

        public RequireDefaultWorkspaceRootHostSetting()
        {
            DefaultValue = DefaultSettingValue;
            Tags = SettingTags.Advanced;
        }
    }
}
