namespace Rubberduck.InternalApi.Settings.Model.Logging;

/// <summary>
/// Allow the minimum log level to remain at a verbose TRACE level after a successful initialization and shutdown; logging normally gets automatically disabled otherwise.
/// </summary>
public record class DisableInitialLogLevelResetSetting : BooleanRubberduckSetting
{
    public static bool DefaultSettingValue { get; } = false;

    public DisableInitialLogLevelResetSetting()
    {
        DefaultValue = DefaultSettingValue;
        Tags = SettingTags.Hidden;
    }
}
