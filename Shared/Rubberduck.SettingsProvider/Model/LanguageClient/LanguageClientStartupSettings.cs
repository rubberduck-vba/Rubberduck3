using Rubberduck.InternalApi.ServerPlatform;
using Rubberduck.SettingsProvider.Model.LanguageServer;
using Rubberduck.SettingsProvider.Model.ServerStartup;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rubberduck.SettingsProvider.Model.LanguageClient
{
    public record class LanguageClientStartupSettings : ServerStartupSettings
    {
        // TODO localize
        private static readonly string _description = "Configures the command-line startup options of the Rubberduck Editor LSP client.";
        private static readonly RubberduckSetting[] DefaultSettings = GetDefaultSettings(nameof(LanguageServerStartupSettings),
            ServerPlatformSettings.EditorServerDefaultPipeName,
            @$"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Rubberduck\Editor\{ServerPlatformSettings.EditorServerExecutable}");

        public LanguageClientStartupSettings()
            : base(nameof(LanguageClientStartupSettings), DefaultSettings, DefaultSettings) { }

        public LanguageClientStartupSettings(params RubberduckSetting[] settings)
            : base(nameof(LanguageClientStartupSettings), settings, DefaultSettings) { }

        public LanguageClientStartupSettings(IEnumerable<RubberduckSetting> settings)
            : base(nameof(LanguageClientStartupSettings), settings.ToArray(), DefaultSettings) { }
    }
}
