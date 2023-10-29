using Rubberduck.InternalApi.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Rubberduck.SettingsProvider.Model.LanguageClient
{
    public record class LanguageClientSettings : TypedSettingGroup, IDefaultSettingsProvider<LanguageClientSettings>
    {
        // TODO localize
        private static readonly string _description = "Configures LSP (Language Server Protocol) client options. The LSP client runs in the Rubberduck Editor process.";
        private static readonly RubberduckSetting[] DefaultSettings =
            new RubberduckSetting[]
            {
                new DefaultWorkspaceRootSetting(),
                new RequireAddInHostSetting(),
                new RequireSavedHostSetting(),
                new RequireDefaultWorkspaceRootHostSetting(),
                new EnableUncWorkspacesSetting(),
                new LanguageClientStartupSettings(),
            };

        public LanguageClientSettings()
            : base(nameof(LanguageClientSettings), DefaultSettings, DefaultSettings) { }

        public LanguageClientSettings(LanguageClientSettings original, IEnumerable<RubberduckSetting> settings)
            : base(original)
        {
            Value = settings?.ToArray() ?? DefaultSettings;
        }

        public LanguageClientSettings(params RubberduckSetting[] settings)
            : base(nameof(LanguageClientSettings), settings, DefaultSettings) { }

        public LanguageClientSettings(IEnumerable<RubberduckSetting> settings)
            : base(nameof(LanguageClientSettings), settings, DefaultSettings) { }

        [JsonIgnore]
        public Uri DefaultWorkspaceRoot => GetSetting<DefaultWorkspaceRootSetting>().TypedValue;
        [JsonIgnore]
        public bool RequireAddInHost => GetSetting<RequireAddInHostSetting>().TypedValue;
        [JsonIgnore]
        public bool RequireSavedHost => GetSetting<RequireSavedHostSetting>().TypedValue;
        [JsonIgnore]
        public bool RequireDefaultWorkspaceRootHost => GetSetting<RequireDefaultWorkspaceRootHostSetting>().TypedValue;
        [JsonIgnore]
        public bool EnableUncWorkspaces => GetSetting<EnableUncWorkspacesSetting>().TypedValue;
        [JsonIgnore]
        public LanguageClientStartupSettings StartupSettings => GetSetting<LanguageClientStartupSettings>();

        public static LanguageClientSettings Default { get; } = new(DefaultSettings);
        LanguageClientSettings IDefaultSettingsProvider<LanguageClientSettings>.Default => Default;
    }
}
