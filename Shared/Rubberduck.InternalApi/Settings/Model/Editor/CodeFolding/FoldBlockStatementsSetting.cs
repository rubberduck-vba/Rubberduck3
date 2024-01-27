namespace Rubberduck.InternalApi.Settings.Model.Editor.CodeFolding;

public record class FoldBlockStatementsSetting : BooleanRubberduckSetting
{
    public static bool DefaultSettingValue { get; } = true;

    public FoldBlockStatementsSetting()
    {
        DefaultValue = DefaultSettingValue;
    }
}