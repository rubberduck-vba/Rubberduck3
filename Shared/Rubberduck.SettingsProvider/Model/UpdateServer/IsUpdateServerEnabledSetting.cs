namespace Rubberduck.SettingsProvider.Model.UpdateServer
{
    /// <summary>
    /// Determines whether the update server is enabled at all.
    /// </summary>
    public class IsUpdateServerEnabledSetting : BooleanRubberduckSetting
    {
        public static bool DefaultSettingValue { get; } = true;

        public IsUpdateServerEnabledSetting()
        {
            DefaultValue = DefaultSettingValue;
        }
    }
}
