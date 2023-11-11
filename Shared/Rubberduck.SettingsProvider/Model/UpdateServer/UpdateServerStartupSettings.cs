using Rubberduck.InternalApi.ServerPlatform;
using Rubberduck.SettingsProvider.Model.ServerStartup;
using System;

namespace Rubberduck.SettingsProvider.Model.UpdateServer
{
    /// <summary>
    /// Configures the command-line startup options of the update server.
    /// </summary>
    public record class UpdateServerStartupSettings : ServerStartupSettings
    {
        public static readonly RubberduckSetting[] DefaultSettings = GetDefaultSettings(ServerPlatformSettings.UpdateServerDefaultPipeName,
            @$"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Rubberduck\Update");

        public UpdateServerStartupSettings()
        {
            SettingDataType = SettingDataType.SettingGroup;
            DefaultValue = DefaultSettings;
        }
    }
}
