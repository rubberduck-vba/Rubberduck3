namespace Rubberduck.InternalApi.Settings.Model.Editor.CodeFolding;

public record class FoldScopesSetting : BooleanRubberduckSetting
{
    public static bool DefaultSettingValue { get; } = true;

    public FoldScopesSetting()
    {
        DefaultValue = DefaultSettingValue;
    }
}
