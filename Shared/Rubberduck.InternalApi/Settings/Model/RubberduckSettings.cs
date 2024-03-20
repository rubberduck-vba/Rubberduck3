using Rubberduck.InternalApi.Settings;
using Rubberduck.InternalApi.Settings.Model.Editor;
using Rubberduck.InternalApi.Settings.Model.General;
using Rubberduck.InternalApi.Settings.Model.LanguageClient;
using Rubberduck.InternalApi.Settings.Model.LanguageServer;
using Rubberduck.InternalApi.Settings.Model.Logging;
using Rubberduck.InternalApi.Settings.Model.TelemetryServer;
using Rubberduck.InternalApi.Settings.Model.UpdateServer;
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
}
