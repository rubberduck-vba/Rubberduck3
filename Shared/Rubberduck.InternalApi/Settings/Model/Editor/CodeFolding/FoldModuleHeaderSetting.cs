namespace Rubberduck.InternalApi.Settings.Model.Editor.CodeFolding;

public record class FoldModuleHeaderSetting : BooleanRubberduckSetting
{
    public static bool DefaultSettingValue { get; } = true;

    public FoldModuleHeaderSetting()
    {
        DefaultValue = DefaultSettingValue;
    }
}
