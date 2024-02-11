namespace Rubberduck.InternalApi.Settings.Model.Editor.CodeFolding;

public record class FoldModuleAttributesSetting : BooleanRubberduckSetting
{
    public static bool DefaultSettingValue { get; } = true;

    public FoldModuleAttributesSetting()
    {
        DefaultValue = DefaultSettingValue;
    }
}
