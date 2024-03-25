using Rubberduck.InternalApi.Settings;
using Rubberduck.InternalApi.Settings.Model.Editor;
using Rubberduck.InternalApi.Settings.Model.General;
using Rubberduck.InternalApi.Settings.Model.LanguageClient;
using Rubberduck.InternalApi.Settings.Model.LanguageServer;
using Rubberduck.InternalApi.Settings.Model.Logging;
using Rubberduck.InternalApi.Settings.Model.TelemetryServer;
using Rubberduck.InternalApi.Settings.Model.UpdateServer;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.Settings.Model;

/// <summary>
/// A container for all configuration settings.
/// </summary>
public record class RubberduckSettings : TypedSettingGroup, IDefaultSettingsProvider<RubberduckSettings>
{
    private static readonly RubberduckSetting[] DefaultSettings =
        [
            GeneralSettings.Default,
            LoggingSettings.Default,
            EditorSettings.Default,
            LanguageClientSettings.Default,
            LanguageServerSettings.Default,
            UpdateServerSettings.Default,
            TelemetryServerSettings.Default,
        ];

    public RubberduckSettings()
    {
        SettingDataType = SettingDataType.SettingGroup;
        Value = DefaultValue = DefaultSettings;
    }

    [JsonIgnore]
    public GeneralSettings GeneralSettings => GetSetting<GeneralSettings>() ?? GeneralSettings.Default;
    [JsonIgnore]
    public LoggingSettings LoggerSettings => GetSetting<LoggingSettings>() ?? LoggingSettings.Default;
    [JsonIgnore]
    public EditorSettings EditorSettings => GetSetting<EditorSettings>() ?? EditorSettings.Default;
    [JsonIgnore]
    public LanguageClientSettings LanguageClientSettings => GetSetting<LanguageClientSettings>() ?? LanguageClientSettings.Default;
    [JsonIgnore]
    public LanguageServerSettings LanguageServerSettings => GetSetting<LanguageServerSettings>() ?? LanguageServerSettings.Default;
    [JsonIgnore]
    public UpdateServerSettings UpdateServerSettings => GetSetting<UpdateServerSettings>() ?? UpdateServerSettings.Default;
    [JsonIgnore]
    public TelemetryServerSettings TelemetryServerSettings => GetSetting<TelemetryServerSettings>() ?? TelemetryServerSettings.Default;

    public static RubberduckSettings Default { get; } = new() { Value = DefaultSettings };
    RubberduckSettings IDefaultSettingsProvider<RubberduckSettings>.Default => Default;

    public RubberduckSettings WithKey(string key) => this with { Key = key };

    public IEnumerable<RubberduckSettingDiff> Diff(RubberduckSettings other)
    {
        var debugFlatSettings = Flatten(this);

        var reference = Flatten(this).ToDictionary(e => e.UniqueKey);
        var comparable = Flatten(other).ToDictionary(e => e.UniqueKey);

        var addedKeys = comparable.Keys.Except(reference.Keys);
        var deletedKeys = reference.Keys.Except(comparable.Keys);
        var modifiedSettings = reference.Where(kvp => 
            reference[kvp.Key].SettingDataType != SettingDataType.EnumSettingGroup 
            && reference[kvp.Key].SettingDataType != SettingDataType.SettingGroup
            && reference[kvp.Key].SettingDataType != SettingDataType.ListSetting
            && IsModified(reference[kvp.Key], comparable[kvp.Key]));

        return addedKeys.Select(e => new RubberduckSettingDiff { ComparableValue = comparable[e] })
            .Concat(deletedKeys.Select(e => new RubberduckSettingDiff { ReferenceValue = reference[e] }))
            .Concat(modifiedSettings.Select(e => new RubberduckSettingDiff { ReferenceValue = reference[e.Key], ComparableValue = comparable[e.Key] }));
    }

    private bool IsModified(RubberduckSetting reference, RubberduckSetting comparable)
    {
        var referenceValue = reference.Value;
        var comparableValue = comparable.Value;
        return !Equals(referenceValue, comparableValue);
    }
}

public record class RubberduckSettingDiff
{
    public string Key => (ReferenceValue ?? ComparableValue)!.Key;
    public SettingDataType SettingDataType => (ReferenceValue ?? ComparableValue)!.SettingDataType;

    public RubberduckSetting? ReferenceValue { get; init; }
    public RubberduckSetting? ComparableValue { get; init; }
}