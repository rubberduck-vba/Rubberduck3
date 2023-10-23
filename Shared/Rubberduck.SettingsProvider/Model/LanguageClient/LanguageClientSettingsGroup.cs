using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model.LanguageServer;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Rubberduck.SettingsProvider.Model.LanguageClient
{
    public interface ILanguageClientSettings
    {
        Uri DefaultWorkspaceRoot { get; }
        bool RequireAddInHost { get; }
        bool RequireSavedHost { get; }
        bool RequireDefaultWorkspaceRootHost { get; }
        bool EnableUncWorkspaces { get; }
        LanguageServerStartupSettings StartupSettings { get; }
    }

    public record class LanguageClientSettingsGroup : SettingGroup, IDefaultSettingsProvider<LanguageClientSettingsGroup>, ILanguageClientSettings
    {
        public static LanguageClientSettingsGroup Default { get; } = new();
        LanguageClientSettingsGroup IDefaultSettingsProvider<LanguageClientSettingsGroup>.Default => Default;

        // TODO localize
        private static readonly string _description = "Configures LSP (Language Server Protocol) client options. The LSP client runs in the Rubberduck Editor process.";

        public LanguageClientSettingsGroup() : base(nameof(LanguageClientSettingsGroup), _description) { }

        public Uri DefaultWorkspaceRoot => new(Values[nameof(DefaultWorkspaceRootSetting)]);
        public bool RequireAddInHost => bool.Parse(Values[nameof(RequireAddInHostSetting)]);
        public bool RequireSavedHost => bool.Parse(Values[nameof(RequireSavedHostSetting)]);
        public bool RequireDefaultWorkspaceRootHost => bool.Parse(Values[nameof(RequireDefaultWorkspaceRootHostSetting)]);
        public bool EnableUncWorkspaces => bool.Parse(Values[nameof(EnableUncWorkspacesSetting)]);
        public LanguageServerStartupSettings StartupSettings => JsonSerializer.Deserialize<LanguageServerStartupSettings>(Values[nameof(StartupSettings)]) ?? new();

        protected override IEnumerable<RubberduckSetting> Settings { get; init; } = new RubberduckSetting[]
        {
            new DefaultWorkspaceRootSetting(),
            new RequireAddInHostSetting(),
            new RequireSavedHostSetting(),
            new RequireDefaultWorkspaceRootHostSetting(),
            new EnableUncWorkspacesSetting(),
        };
    }
}
