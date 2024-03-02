namespace Rubberduck.InternalApi.Settings.Model.Editor.CodeFolding;

public record class FoldRegionsSetting : BooleanRubberduckSetting
{
    public static bool DefaultSettingValue { get; } = true;

    public FoldRegionsSetting()
    {
        DefaultValue = DefaultSettingValue;
    }
}
