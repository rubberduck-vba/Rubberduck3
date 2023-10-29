using Rubberduck.InternalApi.ServerPlatform;
using Rubberduck.SettingsProvider.Model.ServerStartup;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rubberduck.SettingsProvider.Model
{
    public record class UpdateServerStartupSettings : ServerStartupSettings
    {
        // TODO localize
        private static readonly string _description = "Configures the command-line startup options of the update server.";
        private static readonly IRubberduckSetting[] DefaultSettings = GetDefaultSettings(nameof(UpdateServerStartupSettings),
            ServerPlatformSettings.UpdateServerDefaultPipeName,
            @$"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Rubberduck\Update\{ServerPlatformSettings.UpdateServerExecutable}");

        public UpdateServerStartupSettings() 
            : base(nameof(UpdateServerStartupSettings), DefaultSettings, DefaultSettings) { }

        public UpdateServerStartupSettings(params IRubberduckSetting[] settings)
            : base(nameof(UpdateServerStartupSettings), settings, DefaultSettings) { }

        public UpdateServerStartupSettings(IEnumerable<IRubberduckSetting> settings)
            : base(nameof(UpdateServerStartupSettings), settings.ToArray(), DefaultSettings) { }

    }
}
