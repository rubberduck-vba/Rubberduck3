namespace Rubberduck.InternalApi.Settings.Model.Editor.CodeFolding;

public record class CodeFoldingSettings : TypedSettingGroup, IDefaultSettingsProvider<CodeFoldingSettings>
{
    private static readonly RubberduckSetting[] DefaultSettings =
        [
            new FoldModuleHeaderSetting(),
            new FoldModuleAttributesSetting(),
            new FoldModuleDeclarationsSetting(),
            new FoldScopesSetting(),
            new FoldBlockStatementsSetting(),
        ];

    public CodeFoldingSettings()
    {
        SettingDataType = SettingDataType.SettingGroup;
        Value = DefaultValue = DefaultSettings;
    }

    public bool FoldModuleHeader => GetSetting<FoldModuleHeaderSetting>()?.TypedValue ?? FoldModuleHeaderSetting.DefaultSettingValue;
    public bool FoldModuleAttributes => GetSetting<FoldModuleAttributesSetting>()?.TypedValue ?? FoldModuleAttributesSetting.DefaultSettingValue;
    public bool FoldModuleDeclarations => GetSetting<FoldModuleDeclarationsSetting>()?.TypedValue ?? FoldModuleDeclarationsSetting.DefaultSettingValue;
    public bool FoldScopes => GetSetting<FoldScopesSetting>()?.TypedValue ?? FoldScopesSetting.DefaultSettingValue;
    public bool FoldBlockStatements => GetSetting<FoldBlockStatementsSetting>()?.TypedValue ?? FoldScopesSetting.DefaultSettingValue;

    public static CodeFoldingSettings Default { get; } = new() { Value = DefaultSettings };
    CodeFoldingSettings IDefaultSettingsProvider<CodeFoldingSettings>.Default => Default;
}
