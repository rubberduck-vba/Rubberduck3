using System.Text.Json.Serialization;

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

    [JsonIgnore]
    public bool FoldModuleHeader => GetSetting<FoldModuleHeaderSetting>()?.TypedValue ?? FoldModuleHeaderSetting.DefaultSettingValue;
    [JsonIgnore]
    public bool FoldModuleAttributes => GetSetting<FoldModuleAttributesSetting>()?.TypedValue ?? FoldModuleAttributesSetting.DefaultSettingValue;
    [JsonIgnore]
    public bool FoldModuleDeclarations => GetSetting<FoldModuleDeclarationsSetting>()?.TypedValue ?? FoldModuleDeclarationsSetting.DefaultSettingValue;
    [JsonIgnore]
    public bool FoldScopes => GetSetting<FoldScopesSetting>()?.TypedValue ?? FoldScopesSetting.DefaultSettingValue;
    [JsonIgnore]
    public bool FoldBlockStatements => GetSetting<FoldBlockStatementsSetting>()?.TypedValue ?? FoldScopesSetting.DefaultSettingValue;

    public static CodeFoldingSettings Default { get; } = new() { Value = DefaultSettings, DefaultValue = DefaultSettings };
    CodeFoldingSettings IDefaultSettingsProvider<CodeFoldingSettings>.Default => Default;
}
