namespace Rubberduck.InternalApi.Settings.Model.Editor.CodeFolding;

public record class FoldModuleDeclarationsSetting : BooleanRubberduckSetting
{
    public static bool DefaultSettingValue { get; } = true;

    public FoldModuleDeclarationsSetting()
    {
        DefaultValue = DefaultSettingValue;
    }
}
