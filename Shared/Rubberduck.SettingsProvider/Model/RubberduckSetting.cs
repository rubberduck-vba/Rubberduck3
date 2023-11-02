using Rubberduck.SettingsProvider.Model.General;
using Rubberduck.SettingsProvider.Model.LanguageClient;
using Rubberduck.SettingsProvider.Model.LanguageServer;
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

    [JsonDerivedType(typeof(GeneralSettings), nameof(GeneralSettings))]
    [JsonDerivedType(typeof(DisableInitialLegacyIndenterCheckSetting), nameof(DisableInitialLegacyIndenterCheckSetting))]
    [JsonDerivedType(typeof(DisableInitialLogLevelResetSetting), nameof(DisableInitialLogLevelResetSetting))]
    [JsonDerivedType(typeof(LocaleSetting), nameof(LocaleSetting))]
    [JsonDerivedType(typeof(LogLevelSetting), nameof(LogLevelSetting))]
    [JsonDerivedType(typeof(ShowSplashSetting), nameof(ShowSplashSetting))]

    [JsonDerivedType(typeof(LanguageClientSettings), nameof(LanguageClientSettings))]
    [JsonDerivedType(typeof(DefaultWorkspaceRootSetting), nameof(DefaultWorkspaceRootSetting))]
    [JsonDerivedType(typeof(DisabledMessageKeysSetting), nameof(DisabledMessageKeysSetting))]
    [JsonDerivedType(typeof(EnableUncWorkspacesSetting), nameof(EnableUncWorkspacesSetting))]
    [JsonDerivedType(typeof(RequireAddInHostSetting), nameof(RequireAddInHostSetting))]
    [JsonDerivedType(typeof(RequireDefaultWorkspaceRootHostSetting), nameof(RequireDefaultWorkspaceRootHostSetting))]
    [JsonDerivedType(typeof(RequireSavedHostSetting), nameof(RequireSavedHostSetting))]

    [JsonDerivedType(typeof(LanguageServerSettings), nameof(LanguageServerSettings))]

    [JsonDerivedType(typeof(TelemetryServerSettings), nameof(TelemetryServerSettings))]
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

    [JsonDerivedType(typeof(UpdateServerSettings), nameof(UpdateServerSettings))]
    [JsonDerivedType(typeof(IncludePreReleasesSetting), nameof(IncludePreReleasesSetting))]
    [JsonDerivedType(typeof(IsUpdateServerEnabledSetting), nameof(IsUpdateServerEnabledSetting))]
    [JsonDerivedType(typeof(WebApiBaseUrlSetting), nameof(WebApiBaseUrlSetting))]

    [JsonDerivedType(typeof(TraceLevelSetting), nameof(TraceLevelSetting))]
    [JsonDerivedType(typeof(ServerMessageModeSetting), nameof(ServerMessageModeSetting))]
    [JsonDerivedType(typeof(ServerExecutablePathSetting), nameof(ServerExecutablePathSetting))]
    [JsonDerivedType(typeof(ServerPipeNameSetting), nameof(ServerPipeNameSetting))]
    [JsonDerivedType(typeof(ServerTransportTypeSetting), nameof(ServerTransportTypeSetting))]
    [JsonDerivedType(typeof(ClientHealthCheckIntervalSetting), nameof(ClientHealthCheckIntervalSetting))]

    [JsonDerivedType(typeof(LanguageServerStartupSettings), nameof(LanguageServerStartupSettings))]
    [JsonDerivedType(typeof(LanguageClientStartupSettings), nameof(LanguageClientStartupSettings))]
    [JsonDerivedType(typeof(TelemetryServerStartupSettings), nameof(TelemetryServerStartupSettings))]
    [JsonDerivedType(typeof(UpdateServerStartupSettings), nameof(UpdateServerStartupSettings))]

    public record class RubberduckSetting 
    {
        public RubberduckSetting()
        {
            Key = GetType().Name;
        }

        /// <summary>
        /// The resource key for this setting.
        /// </summary>
        [JsonPropertyOrder(0)]
        public virtual string Key { get; init; }
        /// <summary>
        /// The current value of this setting.
        /// </summary>
        [JsonPropertyOrder(1)]
        public virtual object Value { get; set; }
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
    }
}
