namespace Rubberduck.InternalApi.Settings.Model.General;

/// <summary>
/// Determines whether or not to display a splash screen showing ongoing operations while initializing the Rubberduck Editor.
/// </summary>
public record class ShowSplashSetting : BooleanRubberduckSetting
{
    public static bool DefaultSettingValue { get; } = true;

    public ShowSplashSetting()
    {
        DefaultValue = DefaultSettingValue;
    }
}
