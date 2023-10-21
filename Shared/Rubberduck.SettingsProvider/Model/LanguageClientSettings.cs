using Rubberduck.InternalApi.Settings;
using System;

namespace Rubberduck.SettingsProvider.Model
{
    public readonly record struct LanguageClientSettings : IDefaultSettingsProvider<LanguageClientSettings>
    {
        public static LanguageClientSettings Default { get; } = new()
        {
            DefaultWorkspaceRoot = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Rubberduck", "Workspaces")
        };

        public string DefaultWorkspaceRoot { get; init; }

        public string[] DisabledMessageKeys { get; init; }

        LanguageClientSettings IDefaultSettingsProvider<LanguageClientSettings>.Default => LanguageClientSettings.Default;
    }
}
