using Rubberduck.InternalApi.Settings;
using System;
using System.Text.Json.Serialization;

namespace Rubberduck.SettingsProvider.Model.LanguageClient
{
    public record class WorkspaceSettings : TypedSettingGroup, IDefaultSettingsProvider<WorkspaceSettings>
    {
        private static readonly RubberduckSetting[] DefaultSettings =
            new RubberduckSetting[]
            {
                new DefaultWorkspaceRootSetting { Value = DefaultWorkspaceRootSetting.DefaultSettingValue },
                new RequireDefaultWorkspaceRootHostSetting { Value = RequireDefaultWorkspaceRootHostSetting.DefaultSettingValue },
                new EnableUncWorkspacesSetting { Value = EnableUncWorkspacesSetting.DefaultSettingValue },
                new EnableFileSystemWatchersSetting{ Value = EnableFileSystemWatchersSetting.DefaultSettingValue },
            };

        public WorkspaceSettings()
        {
            DefaultValue = DefaultSettings;
        }

        [JsonIgnore]
        public Uri DefaultWorkspaceRoot => GetSetting<DefaultWorkspaceRootSetting>().TypedValue;
        [JsonIgnore]
        public bool RequireDefaultWorkspaceRootHost => GetSetting<RequireDefaultWorkspaceRootHostSetting>().TypedValue;
        [JsonIgnore]
        public bool EnableUncWorkspaces => GetSetting<EnableUncWorkspacesSetting>().TypedValue;
        [JsonIgnore]
        public bool EnableFileSystemWatchers => GetSetting<EnableFileSystemWatchersSetting>().TypedValue;

        public static WorkspaceSettings Default { get; } = new() { Value = DefaultSettings, DefaultValue = DefaultSettings };
        WorkspaceSettings IDefaultSettingsProvider<WorkspaceSettings>.Default => Default;
    }
}
