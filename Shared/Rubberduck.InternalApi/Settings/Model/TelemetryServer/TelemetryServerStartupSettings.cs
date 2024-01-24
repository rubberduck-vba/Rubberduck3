using Rubberduck.InternalApi.Settings.Model.ServerStartup;
using System;

namespace Rubberduck.InternalApi.Settings.Model.TelemetryServer
{
    /// <summary>
    /// Configures the command-line startup options of the telemetry server.
    /// </summary>
    public record class TelemetryServerStartupSettings : ServerStartupSettings
    {
        public static RubberduckSetting[] DefaultSettings { get; } = GetDefaultSettings(ServerPlatformSettings.TelemetryServerDefaultPipeName,
            @$"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Rubberduck\Telemetry");

        public TelemetryServerStartupSettings()
        {
            SettingDataType = SettingDataType.SettingGroup;
            DefaultValue = DefaultSettings;
        }

        public static TelemetryServerStartupSettings Default { get; } = new() { DefaultValue = DefaultSettings, Value = DefaultSettings };
    }
}
