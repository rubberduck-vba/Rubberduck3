using Rubberduck.InternalApi.ServerPlatform;
using Rubberduck.SettingsProvider.Model.ServerStartup;
using System;
using System.Collections.Generic;

namespace Rubberduck.SettingsProvider.Model.LanguageServer
{
    public record class LanguageServerStartupSettings : ServerStartupSettings
    {
        // TODO localize
        private static readonly string _description = "Configures the command-line startup options of the language server.";

        private static readonly IRubberduckSetting[] DefaultSettings = GetDefaultSettings(nameof(LanguageServerStartupSettings),
            ServerPlatformSettings.LanguageServerDefaultPipeName,
            @$"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Rubberduck\LanguageServer\{ServerPlatformSettings.LanguageServerExecutable}");

        public LanguageServerStartupSettings()
            : base(nameof(LanguageServerStartupSettings), DefaultSettings, DefaultSettings) { }

        public LanguageServerStartupSettings(params IRubberduckSetting[] settings)
            : base(nameof(LanguageServerStartupSettings), settings, DefaultSettings) { }

        public LanguageServerStartupSettings(IEnumerable<IRubberduckSetting> settings)
            : base(nameof(LanguageServerStartupSettings), settings, DefaultSettings) { }
    }
}
