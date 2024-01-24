using Rubberduck.InternalApi.Settings.Model.ServerStartup;
using System;

namespace Rubberduck.InternalApi.Settings.Model.LanguageServer
{
    /// <summary>
    /// Configures the command-line startup options of the language server.
    /// </summary>
    public record class LanguageServerStartupSettings : ServerStartupSettings
    {
        public static readonly RubberduckSetting[] DefaultSettings = GetDefaultSettings(ServerPlatformSettings.LanguageServerDefaultPipeName,
            @$"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Rubberduck\LanguageServer");

        public LanguageServerStartupSettings()
        {
            DefaultValue = DefaultSettings;
        }

        public static LanguageServerStartupSettings Default { get; } = new() { DefaultValue = DefaultSettings, Value = DefaultSettings };
    }
}
