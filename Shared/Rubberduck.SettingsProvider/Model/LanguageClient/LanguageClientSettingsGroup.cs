using Rubberduck.InternalApi.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
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
        LanguageClientStartupSettings StartupSettings { get; }
    }

    public record class LanguageClientSettingsGroup : SettingGroup, IDefaultSettingsProvider<LanguageClientSettingsGroup>, ILanguageClientSettings
    {
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

        public static LanguageClientSettingsGroup Default { get; } = new(DefaultSettings);
        LanguageClientSettingsGroup IDefaultSettingsProvider<LanguageClientSettingsGroup>.Default => Default;

        // TODO localize
        private static readonly string _description = "Configures LSP (Language Server Protocol) client options. The LSP client runs in the Rubberduck Editor process.";

        public LanguageClientSettingsGroup(LanguageClientSettingsGroup original, IEnumerable<RubberduckSetting> settings)
            : base(original)
        {
            var values = original.Settings.ToDictionary(e => e.Name);
            if (settings != null)
            {
                foreach (var setting in settings)
                {
                    values[setting.Name] = setting;
                }
                settings = values.Values;
            }

            Settings = settings ?? DefaultSettings;
            DefaultWorkspaceRoot = Settings.OfType<DefaultWorkspaceRootSetting>().Single().Value;
            RequireAddInHost = Settings.OfType<RequireAddInHostSetting>().Single().Value;
            RequireSavedHost = Settings.OfType<RequireSavedHostSetting>().Single().Value;
            RequireDefaultWorkspaceRootHost = Settings.OfType<RequireDefaultWorkspaceRootHostSetting>().Single().Value;
            EnableUncWorkspaces = Settings.OfType<EnableUncWorkspacesSetting>().Single().Value;
            StartupSettings = Settings.OfType<LanguageClientStartupSettings>().Single();
        }

        public LanguageClientSettingsGroup(IEnumerable<RubberduckSetting> settings)
            : base(nameof(LanguageClientSettingsGroup), _description) 
        {
            Settings = settings ?? DefaultSettings;
            DefaultWorkspaceRoot = Settings.OfType<DefaultWorkspaceRootSetting>().Single().Value;
            RequireAddInHost = Settings.OfType<RequireAddInHostSetting>().Single().Value;
            RequireSavedHost = Settings.OfType<RequireSavedHostSetting>().Single().Value;
            RequireDefaultWorkspaceRootHost = Settings.OfType<RequireDefaultWorkspaceRootHostSetting>().Single().Value;
            EnableUncWorkspaces = Settings.OfType<EnableUncWorkspacesSetting>().Single().Value;
            StartupSettings = Settings.OfType<LanguageClientStartupSettings>().Single();
        }

        public LanguageClientSettingsGroup() : base(nameof(LanguageClientSettingsGroup), _description) { }

        public Uri DefaultWorkspaceRoot { get; init; }
        public bool RequireAddInHost { get; init; }
        public bool RequireSavedHost { get; init; }
        public bool RequireDefaultWorkspaceRootHost { get; init; }
        public bool EnableUncWorkspaces { get; init; }
        public LanguageClientStartupSettings StartupSettings { get; init; }
    }
}
