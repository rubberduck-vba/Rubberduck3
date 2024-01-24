namespace Rubberduck.InternalApi.Settings.Model.General
{
    /// <summary>
    /// Allow Rubberduck to scan the registry for legacy Smart Indenter settings after a successful initialization and shutdown; this initial check normally gets automatically disabled otherwise.
    /// </summary>
    public record class DisableInitialLegacyIndenterCheckSetting : BooleanRubberduckSetting
    {
        public static bool DefaultSettingValue { get; } = false;

        public DisableInitialLegacyIndenterCheckSetting()
        {
            DefaultValue = DefaultSettingValue;
            Tags = SettingTags.Hidden;
        }
    }
}
