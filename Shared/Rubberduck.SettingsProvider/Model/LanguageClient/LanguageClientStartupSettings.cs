using Rubberduck.InternalApi.ServerPlatform;
using Rubberduck.SettingsProvider.Model.LanguageServer;
using Rubberduck.SettingsProvider.Model.ServerStartup;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rubberduck.SettingsProvider.Model.LanguageClient
{
    /// <summary>
    /// Configures the command-line startup options of the Rubberduck Editor LSP client.
    /// </summary>
    public class LanguageClientStartupSettings : ServerStartupSettings
    {
        public static RubberduckSetting[] DefaultSettings { get; }  = GetDefaultSettings(ServerPlatformSettings.EditorServerDefaultPipeName,
            @$"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Rubberduck\Editor\{ServerPlatformSettings.EditorServerExecutable}");

        public LanguageClientStartupSettings()
        {
            DefaultValue = DefaultSettings;
        }
    }
}
