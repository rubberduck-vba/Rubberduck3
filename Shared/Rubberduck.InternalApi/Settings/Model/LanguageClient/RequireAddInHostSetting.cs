namespace Rubberduck.InternalApi.Settings.Model.LanguageClient;

/// <summary>
/// Whether the Rubberduck Editor is allowed to run without a VBIDE-connected Rubberduck add-in host; disables host-dependent features when false.
/// </summary>
public record class RequireAddInHostSetting : BooleanRubberduckSetting
{
    public static bool DefaultSettingValue { get; } = false;

    public RequireAddInHostSetting()
    {
        DefaultValue = DefaultSettingValue;
        Tags = SettingTags.Advanced | SettingTags.ReadOnlyRecommended | SettingTags.Experimental;
    }
}
