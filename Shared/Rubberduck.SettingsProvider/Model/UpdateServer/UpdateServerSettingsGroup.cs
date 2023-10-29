using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model.ServerStartup;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rubberduck.SettingsProvider.Model
{
    public interface IUpdateServerSettings
    {
        MessageTraceLevel TraceLevel { get; }
        bool IsEnabled { get; }
        bool IncludePreReleases { get; }
        Uri RubberduckWebApiBaseUrl { get; }
    }

    public record class UpdateServerSettingsGroup : SettingGroup, IDefaultSettingsProvider<UpdateServerSettingsGroup>, IUpdateServerSettings
    {
        // TODO localize
        private static readonly string _description = "Configures the update server settings.";
        public static IRubberduckSetting[] DefaultSettings = new IRubberduckSetting[]
        {
            new TraceLevelSetting(nameof(TraceLevelSetting), MessageTraceLevel.Verbose),
            new IsUpdateServerEnabledSetting(true),
            new IncludePreReleasesSetting(true),
            new WebApiBaseUrlSetting(new Uri("https://api.rubberduckvba.com/api/v1")),
        };


        public UpdateServerSettingsGroup() 
            : base(nameof(UpdateServerSettingsGroup), DefaultSettings, DefaultSettings){ }

        public UpdateServerSettingsGroup(params IRubberduckSetting[] settings)
            : base(nameof(UpdateServerSettingsGroup), settings, DefaultSettings) { }

        public UpdateServerSettingsGroup(IEnumerable<IRubberduckSetting> settings)
            : base(nameof(UpdateServerSettingsGroup), settings, DefaultSettings) { }


        public MessageTraceLevel TraceLevel => GetSetting<TraceLevelSetting>().Value;
        public bool IsEnabled => GetSetting<IsUpdateServerEnabledSetting>().Value;
        public bool IncludePreReleases => GetSetting<IncludePreReleasesSetting>().Value;
        public Uri RubberduckWebApiBaseUrl => GetSetting<WebApiBaseUrlSetting>().Value;

        public static UpdateServerSettingsGroup Default { get; } = new(DefaultSettings);
        UpdateServerSettingsGroup IDefaultSettingsProvider<UpdateServerSettingsGroup>.Default => Default;
    }
}
