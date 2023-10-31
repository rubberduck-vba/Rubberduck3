using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model.ServerStartup;
using System.Text.Json.Serialization;

namespace Rubberduck.SettingsProvider.Model.LanguageServer
{
    /// <summary>
    /// Configures LSP (Language Server Protocol) server options.
    /// </summary>
    public record class LanguageServerSettings : TypedSettingGroup, IDefaultSettingsProvider<LanguageServerSettings>
    {
        private static readonly RubberduckSetting[] DefaultSettings =
            new RubberduckSetting[]
            {
                new TraceLevelSetting{ Value = TraceLevelSetting.DefaultSettingValue },
                new LanguageServerStartupSettings { Value = LanguageServerStartupSettings.DefaultSettings },
            };

        public LanguageServerSettings()
        {
            DefaultValue = DefaultSettings;
        }

        [JsonIgnore]
        public MessageTraceLevel TraceLevel => GetSetting<TraceLevelSetting>().TypedValue;
        [JsonIgnore]
        public LanguageServerStartupSettings StartupSettings => GetSetting<LanguageServerStartupSettings>();

        public static LanguageServerSettings Default { get; } = new() { Value = DefaultSettings, DefaultValue = DefaultSettings };
        LanguageServerSettings IDefaultSettingsProvider<LanguageServerSettings>.Default => Default;
    }
}
