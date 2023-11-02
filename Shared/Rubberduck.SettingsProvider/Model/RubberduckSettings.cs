using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model.General;
using Rubberduck.SettingsProvider.Model.LanguageClient;
using Rubberduck.SettingsProvider.Model.LanguageServer;
using Rubberduck.SettingsProvider.Model.TelemetryServer;
using Rubberduck.SettingsProvider.Model.UpdateServer;
using System.Text.Json.Serialization;

namespace Rubberduck.SettingsProvider.Model
{
    /// <summary>
    /// A container for all configuration settings.
    /// </summary>
    public record class RubberduckSettings : TypedSettingGroup, IDefaultSettingsProvider<RubberduckSettings>
    {
        private static readonly RubberduckSetting[] DefaultSettings =
            new RubberduckSetting[]
            {
                GeneralSettings.Default,
                LanguageClientSettings.Default,
                LanguageServerSettings.Default,
                UpdateServerSettings.Default,
                TelemetryServerSettings.Default,
            };

        public RubberduckSettings() 
        {
            SettingDataType = SettingDataType.SettingGroup;
            DefaultValue = DefaultSettings;
        }

        [JsonIgnore]
        public GeneralSettings GeneralSettings => GetSetting<GeneralSettings>();
        [JsonIgnore]
        public LanguageClientSettings LanguageClientSettings => GetSetting<LanguageClientSettings>();
        [JsonIgnore]
        public LanguageServerSettings LanguageServerSettings => GetSetting<LanguageServerSettings>();
        [JsonIgnore]
        public UpdateServerSettings UpdateServerSettings => GetSetting<UpdateServerSettings>();
        [JsonIgnore]
        public TelemetryServerSettings TelemetryServerSettings => GetSetting<TelemetryServerSettings>();

        public static RubberduckSettings Default { get; } = new() { Value = DefaultSettings };
        RubberduckSettings IDefaultSettingsProvider<RubberduckSettings>.Default => Default;
    }
}
