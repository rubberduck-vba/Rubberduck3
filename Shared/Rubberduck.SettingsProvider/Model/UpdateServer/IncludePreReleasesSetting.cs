namespace Rubberduck.SettingsProvider.Model.UpdateServer
{
    /// <summary>
    /// Determines whether checking for a newer version includes or ignores pre-release builds.
    /// </summary>
    public class IncludePreReleasesSetting : BooleanRubberduckSetting
    {
        public static bool DefaultSettingValue { get; } = true;

        public IncludePreReleasesSetting()
        {
            DefaultValue = DefaultSettingValue;
        }
    }
}
