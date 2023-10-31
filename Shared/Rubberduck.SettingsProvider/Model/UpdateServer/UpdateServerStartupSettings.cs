using Rubberduck.InternalApi.ServerPlatform;
using Rubberduck.SettingsProvider.Model.ServerStartup;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rubberduck.SettingsProvider.Model.UpdateServer
{
    /// <summary>
    /// Configures the command-line startup options of the update server.
    /// </summary>
    public class UpdateServerStartupSettings : ServerStartupSettings
    {
        public static readonly RubberduckSetting[] DefaultSettings = GetDefaultSettings(ServerPlatformSettings.UpdateServerDefaultPipeName,
            @$"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Rubberduck\Update\{ServerPlatformSettings.UpdateServerExecutable}");

        public UpdateServerStartupSettings()
        {
            SettingDataType = SettingDataType.SettingGroup;
            DefaultValue = DefaultSettings;
        }
    }
}
