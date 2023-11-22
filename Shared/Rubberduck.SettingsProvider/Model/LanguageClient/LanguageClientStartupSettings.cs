using Rubberduck.InternalApi.ServerPlatform;
using Rubberduck.SettingsProvider.Model.ServerStartup;
using System;

namespace Rubberduck.SettingsProvider.Model.LanguageClient
{
    /// <summary>
    /// Configures the command-line startup options of the Rubberduck Editor LSP client.
    /// </summary>
    public record class LanguageClientStartupSettings : ServerStartupSettings
    {
        public static RubberduckSetting[] DefaultSettings { get; }  = GetDefaultSettings(ServerPlatformSettings.EditorServerDefaultPipeName,
            @$"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Rubberduck\Editor");

        public LanguageClientStartupSettings()
        {
            DefaultValue = DefaultSettings;
        }

        public static LanguageClientStartupSettings Default { get; } = new() { Value = DefaultSettings, DefaultValue = DefaultSettings };
    }
}
