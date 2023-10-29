using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model.ServerStartup;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Rubberduck.SettingsProvider.Model
{
    public record class UpdateServerSettings : TypedSettingGroup, IDefaultSettingsProvider<UpdateServerSettings>
    {
        // TODO localize
        private static readonly string _description = "Configures the update server settings.";
        public static RubberduckSetting[] DefaultSettings = new RubberduckSetting[]
        {
            new UpdateServerStartupSettings(),
            new TraceLevelSetting(nameof(TraceLevelSetting), MessageTraceLevel.Verbose),
            new IsUpdateServerEnabledSetting(true),
            new IncludePreReleasesSetting(true),
            new WebApiBaseUrlSetting(new Uri("https://api.rubberduckvba.com/api/v1")),
        };


        public UpdateServerSettings() 
            : base(nameof(UpdateServerSettings), DefaultSettings, DefaultSettings){ }

        public UpdateServerSettings(params RubberduckSetting[] settings)
            : base(nameof(UpdateServerSettings), settings, DefaultSettings) { }

        public UpdateServerSettings(IEnumerable<RubberduckSetting> settings)
            : base(nameof(UpdateServerSettings), settings, DefaultSettings) { }


        [JsonIgnore]
        public UpdateServerStartupSettings StartupSettings => GetSetting<UpdateServerStartupSettings>();
        [JsonIgnore]
        public MessageTraceLevel TraceLevel => GetSetting<TraceLevelSetting>().TypedValue;
        [JsonIgnore]
        public bool IsEnabled => GetSetting<IsUpdateServerEnabledSetting>().TypedValue;
        [JsonIgnore]
        public bool IncludePreReleases => GetSetting<IncludePreReleasesSetting>().TypedValue;
        [JsonIgnore]
        public Uri RubberduckWebApiBaseUrl => GetSetting<WebApiBaseUrlSetting>().TypedValue;

        public static UpdateServerSettings Default { get; } = new(DefaultSettings);
        UpdateServerSettings IDefaultSettingsProvider<UpdateServerSettings>.Default => Default;
    }
}
