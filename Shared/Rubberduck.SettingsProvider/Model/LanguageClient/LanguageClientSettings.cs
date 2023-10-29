using Rubberduck.InternalApi.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rubberduck.SettingsProvider.Model.LanguageClient
{
    public record class LanguageClientSettings : SettingGroup, IDefaultSettingsProvider<LanguageClientSettings>
    {
        // TODO localize
        private static readonly string _description = "Configures LSP (Language Server Protocol) client options. The LSP client runs in the Rubberduck Editor process.";
        private static readonly IRubberduckSetting[] DefaultSettings =
            new IRubberduckSetting[]
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

        public LanguageClientSettings(LanguageClientSettings original, IEnumerable<IRubberduckSetting> settings)
            : base(original)
        {
            Value = settings?.ToArray() ?? DefaultSettings;
        }

        public LanguageClientSettings(params IRubberduckSetting[] settings)
            : base(nameof(LanguageClientSettings), settings, DefaultSettings) { }

        public LanguageClientSettings(IEnumerable<IRubberduckSetting> settings)
            : base(nameof(LanguageClientSettings), settings, DefaultSettings) { }

        public Uri DefaultWorkspaceRoot => GetSetting<DefaultWorkspaceRootSetting>().Value;
        public bool RequireAddInHost => GetSetting<RequireAddInHostSetting>().Value;
        public bool RequireSavedHost => GetSetting<RequireSavedHostSetting>().Value;
        public bool RequireDefaultWorkspaceRootHost => GetSetting<RequireDefaultWorkspaceRootHostSetting>().Value;
        public bool EnableUncWorkspaces => GetSetting<EnableUncWorkspacesSetting>().Value;
        public LanguageClientStartupSettings StartupSettings => GetSetting<LanguageClientStartupSettings>();

        public static LanguageClientSettings Default { get; } = new(DefaultSettings);
        LanguageClientSettings IDefaultSettingsProvider<LanguageClientSettings>.Default => Default;
    }
}
