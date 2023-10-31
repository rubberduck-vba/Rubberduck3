namespace Rubberduck.SettingsProvider.Model.LanguageClient
{
    /// <summary>
    /// Whether the Rubberduck Editor is allowed to run without a VBIDE-connected Rubberduck add-in host; disables host-dependent features when false.
    /// </summary>
    public class RequireAddInHostSetting : BooleanRubberduckSetting
    {
        public static bool DefaultSettingValue { get; } = true;

        public RequireAddInHostSetting()
        {
            DefaultValue = DefaultSettingValue;
            Tags = SettingTags.Advanced | SettingTags.ReadOnlyRecommended | SettingTags.Experimental;
        }
    }
}
