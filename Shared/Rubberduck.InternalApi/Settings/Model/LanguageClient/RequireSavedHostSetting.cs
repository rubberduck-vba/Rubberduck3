namespace Rubberduck.InternalApi.Settings.Model.LanguageClient
{
    /// <summary>
    /// Whether projects are required to be created from a saved host document.
    /// </summary>
    public record class RequireSavedHostSetting : BooleanRubberduckSetting
    {
        public static bool DefaultSettingValue { get; } = true;

        public RequireSavedHostSetting()
        {
            DefaultValue = DefaultSettingValue;
            Tags = SettingTags.Advanced | SettingTags.ReadOnlyRecommended | SettingTags.Experimental;
        }
    }
}
