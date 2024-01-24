namespace Rubberduck.InternalApi.Settings.Model.LanguageClient
{
    /// <summary>
    /// Enables or disables client-side file system watchers to pick up any external workspace changes.
    /// </summary>
    public record class EnableFileSystemWatchersSetting : BooleanRubberduckSetting
    {
        public static bool DefaultSettingValue { get; } = true;

        public EnableFileSystemWatchersSetting()
        {
            DefaultValue = DefaultSettingValue;
            Tags = SettingTags.Advanced;
        }
    }
}
