using Rubberduck.Resources;
using Rubberduck.SettingsProvider.Model.Editor;
using Rubberduck.SettingsProvider.Model.Editor.Tools;
using Rubberduck.SettingsProvider.Model.General;
using Rubberduck.SettingsProvider.Model.LanguageClient;
using Rubberduck.SettingsProvider.Model.LanguageServer;
using Rubberduck.SettingsProvider.Model.Logging;
using Rubberduck.SettingsProvider.Model.ServerStartup;
using Rubberduck.SettingsProvider.Model.TelemetryServer;
using Rubberduck.SettingsProvider.Model.UpdateServer;
using System.Text.Json.Serialization;

namespace Rubberduck.SettingsProvider.Model
{
    /// <summary>
    /// The base type for all settings.
    /// </summary>
    [JsonPolymorphic(UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor)]

    [JsonDerivedType(typeof(RubberduckSettings), nameof(RubberduckSettings))]

    [JsonDerivedType(typeof(GeneralSettings), nameof(GeneralSettings))] // ~ General
    [JsonDerivedType(typeof(DisableInitialLegacyIndenterCheckSetting), nameof(DisableInitialLegacyIndenterCheckSetting))]
    [JsonDerivedType(typeof(DisableInitialLogLevelResetSetting), nameof(DisableInitialLogLevelResetSetting))]
    [JsonDerivedType(typeof(DisabledMessageKeysSetting), nameof(DisabledMessageKeysSetting))]
    [JsonDerivedType(typeof(LocaleSetting), nameof(LocaleSetting))]
    [JsonDerivedType(typeof(LogLevelSetting), nameof(LogLevelSetting))]
    [JsonDerivedType(typeof(ShowSplashSetting), nameof(ShowSplashSetting))]
    [JsonDerivedType(typeof(ExitNotificationDelaySetting), nameof(ExitNotificationDelaySetting))]
    [JsonDerivedType(typeof(TemplatesLocationSetting), nameof(TemplatesLocationSetting))]

    [JsonDerivedType(typeof(LanguageClientSettings), nameof(LanguageClientSettings))] // ~ Language client
    [JsonDerivedType(typeof(RequireAddInHostSetting), nameof(RequireAddInHostSetting))]
    [JsonDerivedType(typeof(RequireSavedHostSetting), nameof(RequireSavedHostSetting))]

    [JsonDerivedType(typeof(WorkspaceSettings), nameof(WorkspaceSettings))] // ~ workspace settings (language client)
    [JsonDerivedType(typeof(DefaultWorkspaceRootSetting), nameof(DefaultWorkspaceRootSetting))]
    [JsonDerivedType(typeof(RequireDefaultWorkspaceRootHostSetting), nameof(RequireDefaultWorkspaceRootHostSetting))]
    [JsonDerivedType(typeof(EnableUncWorkspacesSetting), nameof(EnableUncWorkspacesSetting))]
    [JsonDerivedType(typeof(EnableFileSystemWatchersSetting), nameof(EnableFileSystemWatchersSetting))]

    [JsonDerivedType(typeof(EditorSettings), nameof(EditorSettings))] // ~ Editor
    [JsonDerivedType(typeof(ExtendWindowChromeSetting), nameof(ExtendWindowChromeSetting))]
    [JsonDerivedType(typeof(ShowWelcomeTabSetting), nameof(ShowWelcomeTabSetting))]
    [JsonDerivedType(typeof(ToolsSettings), nameof(ToolsSettings))] // ~ Editor/Tool[windows]s
    [JsonDerivedType(typeof(AutoHideToolWindowSetting), nameof(AutoHideToolWindowSetting))]
    [JsonDerivedType(typeof(DefaultToolWindowLocationSetting), nameof(DefaultToolWindowLocationSetting))]
    [JsonDerivedType(typeof(ShowToolWindowOnStartupSetting), nameof(ShowToolWindowOnStartupSetting))]
    [JsonDerivedType(typeof(ToolWindowSettings), nameof(ToolWindowSettings))]
    [JsonDerivedType(typeof(MaximumMessagesSetting), nameof(MaximumMessagesSetting))]
    [JsonDerivedType(typeof(WorkspaceExplorerSettings), nameof(WorkspaceExplorerSettings))] // ~ workspace explorer

    [JsonDerivedType(typeof(LanguageServerSettings), nameof(LanguageServerSettings))] // ~ Language server

    [JsonDerivedType(typeof(TelemetryServerSettings), nameof(TelemetryServerSettings))] // ~ Telemetry server
    [JsonDerivedType(typeof(IsTelemetryEnabledSetting), nameof(IsTelemetryEnabledSetting))]
    [JsonDerivedType(typeof(SendEventTelemetrySetting), nameof(SendEventTelemetrySetting))]
    [JsonDerivedType(typeof(SendExceptionTelemetrySetting), nameof(SendExceptionTelemetrySetting))]
    [JsonDerivedType(typeof(SendMetricTelemetrySetting), nameof(SendMetricTelemetrySetting))]
    [JsonDerivedType(typeof(SendTraceTelemetrySetting), nameof(SendTraceTelemetrySetting))]
    [JsonDerivedType(typeof(StreamTransmissionSetting), nameof(StreamTransmissionSetting))]
    [JsonDerivedType(typeof(TelemetryEventQueueSizeSetting), nameof(TelemetryEventQueueSizeSetting))]
    [JsonDerivedType(typeof(EventTelemetrySettings), nameof(EventTelemetrySettings))]
    [JsonDerivedType(typeof(ExceptionTelemetrySettings), nameof(ExceptionTelemetrySettings))]
    [JsonDerivedType(typeof(TraceTelemetrySettings), nameof(TraceTelemetrySettings))]
    [JsonDerivedType(typeof(MetricTelemetrySettings), nameof(MetricTelemetrySettings))]

    [JsonDerivedType(typeof(UpdateServerSettings), nameof(UpdateServerSettings))] // ~ Update server
    [JsonDerivedType(typeof(IncludePreReleasesSetting), nameof(IncludePreReleasesSetting))]
    [JsonDerivedType(typeof(IsUpdateServerEnabledSetting), nameof(IsUpdateServerEnabledSetting))]
    [JsonDerivedType(typeof(WebApiBaseUrlSetting), nameof(WebApiBaseUrlSetting))]

    // derived ServerStartup setting groups:
    [JsonDerivedType(typeof(LanguageServerStartupSettings), nameof(LanguageServerStartupSettings))]
    [JsonDerivedType(typeof(LanguageClientStartupSettings), nameof(LanguageClientStartupSettings))]
    [JsonDerivedType(typeof(TelemetryServerStartupSettings), nameof(TelemetryServerStartupSettings))]
    [JsonDerivedType(typeof(UpdateServerStartupSettings), nameof(UpdateServerStartupSettings))]

    // ServerStartup settings:
    [JsonDerivedType(typeof(ClientHealthCheckIntervalSetting), nameof(ClientHealthCheckIntervalSetting))]
    [JsonDerivedType(typeof(ServerExecutablePathSetting), nameof(ServerExecutablePathSetting))]
    [JsonDerivedType(typeof(ServerMessageModeSetting), nameof(ServerMessageModeSetting))]
    [JsonDerivedType(typeof(ServerPipeNameSetting), nameof(ServerPipeNameSetting))]
    [JsonDerivedType(typeof(ServerTransportTypeSetting), nameof(ServerTransportTypeSetting))]
    [JsonDerivedType(typeof(TraceLevelSetting), nameof(TraceLevelSetting))]

    // ...

    public record class RubberduckSetting 
    {
        public RubberduckSetting()
        {
            Key = GetType().Name;
        }

        /// <summary>
        /// The resource key for this setting.
        /// </summary>
        public virtual string Key { get; init; }

        public string LocalizedName => RubberduckUI.ResourceManager.GetString($"{Key}.Name") ?? Key;
        public string LocalizedDescription => RubberduckUI.ResourceManager.GetString($"{Key}.Description") ?? Key;

        /// <summary>
        /// The current value of this setting.
        /// </summary>
        public virtual object? Value { get; set; }
        /// <summary>
        /// The supported data type of the setting value.
        /// </summary>
        [JsonIgnore]
        public SettingDataType SettingDataType { get; init; }
        /// <summary>
        /// Additional flags that describe this setting.
        /// </summary>
        [JsonIgnore]
        public SettingTags Tags { get; init; }

        public RubberduckSetting WithValue(object value) => this with { Value = value };
    }
}
