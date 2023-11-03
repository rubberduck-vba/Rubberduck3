using Rubberduck.InternalApi.ServerPlatform;
using Rubberduck.SettingsProvider.Model.ServerStartup;
using System;

namespace Rubberduck.SettingsProvider.Model.TelemetryServer
{
    /// <summary>
    /// Configures the command-line startup options of the telemetry server.
    /// </summary>
    public record class TelemetryServerStartupSettings : ServerStartupSettings
    {
        public static RubberduckSetting[] DefaultSettings { get; } = GetDefaultSettings(ServerPlatformSettings.TelemetryServerDefaultPipeName,
            @$"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Rubberduck\Telemetry\{ServerPlatformSettings.TelemetryServerExecutable}");

        public TelemetryServerStartupSettings()
        {
            SettingDataType = SettingDataType.SettingGroup;
            DefaultValue = DefaultSettings;
        }
    }
}
