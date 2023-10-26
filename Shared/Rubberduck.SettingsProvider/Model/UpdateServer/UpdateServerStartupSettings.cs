using Rubberduck.InternalApi.ServerPlatform;
using Rubberduck.SettingsProvider.Model.ServerStartup;
using System;

namespace Rubberduck.SettingsProvider.Model
{
    public record class UpdateServerStartupSettings : ServerStartupSettings
    {
        // TODO localize
        private static readonly string _description = "Configures the command-line startup options of the update server.";

        public UpdateServerStartupSettings() 
            : base(nameof(UpdateServerStartupSettings), _description)
        {
        }

        protected override string DefaultServerExecutablePath
            => @$"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Rubberduck\Update\{ServerPlatformSettings.UpdateServerExecutable}";

        protected override string DefaultServerPipeName => ServerPlatformSettings.UpdateServerDefaultPipeName;
    }
}
