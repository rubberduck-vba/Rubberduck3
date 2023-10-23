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
        public static LanguageClientSettingsGroup Default { get; } = new();
        LanguageClientSettingsGroup IDefaultSettingsProvider<LanguageClientSettingsGroup>.Default => Default;

        private static readonly RubberduckSetting[] DefaultSettings =
            new RubberduckSetting[]
            {
                new DefaultWorkspaceRootSetting(),
                new RequireAddInHostSetting(),
                new RequireSavedHostSetting(),
                new RequireDefaultWorkspaceRootHostSetting(),
                new EnableUncWorkspacesSetting(),
            };


        // TODO localize
        private static readonly string _description = "Configures LSP (Language Server Protocol) client options. The LSP client runs in the Rubberduck Editor process.";

        public LanguageClientSettingsGroup(LanguageClientSettingsGroup original, IEnumerable<RubberduckSetting>? settings = null)
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
        }

        public LanguageClientSettingsGroup(IEnumerable<RubberduckSetting>? settings = null)
            : base(nameof(LanguageClientSettingsGroup), _description) 
        {
            Settings = settings ?? DefaultSettings;
        }

        public Uri DefaultWorkspaceRoot => new(Values[nameof(DefaultWorkspaceRootSetting)]);
        public bool RequireAddInHost => bool.Parse(Values[nameof(RequireAddInHostSetting)]);
        public bool RequireSavedHost => bool.Parse(Values[nameof(RequireSavedHostSetting)]);
        public bool RequireDefaultWorkspaceRootHost => bool.Parse(Values[nameof(RequireDefaultWorkspaceRootHostSetting)]);
        public bool EnableUncWorkspaces => bool.Parse(Values[nameof(EnableUncWorkspacesSetting)]);
        public LanguageClientStartupSettings StartupSettings => JsonSerializer.Deserialize<LanguageClientStartupSettings>(Values[nameof(StartupSettings)]) ?? new();

        protected override IEnumerable<RubberduckSetting> Settings { get; init; }
    }
}
