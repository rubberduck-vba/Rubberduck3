using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model.ServerStartup;
using System;
using System.Collections.Generic;

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
        public static UpdateServerSettingsGroup Default { get; } = new();
        UpdateServerSettingsGroup IDefaultSettingsProvider<UpdateServerSettingsGroup>.Default => Default;

        // TODO localize
        private static readonly string _description = "Configures the update server settings.";

        public UpdateServerSettingsGroup() : base(nameof(UpdateServerSettingsGroup), _description) { }

        public MessageTraceLevel TraceLevel => Enum.Parse<MessageTraceLevel>(Values[nameof(TraceLevelSetting)]);

        public bool IsEnabled => bool.Parse(Values[nameof(IsUpdateServerEnabledSetting)]);
        public bool IncludePreReleases => bool.Parse(Values[nameof(IncludePreReleasesSetting)]);
        public Uri RubberduckWebApiBaseUrl => new(Values[nameof(WebApiBaseUrlSetting)]);

        protected override IEnumerable<RubberduckSetting> Settings { get; init; } = new RubberduckSetting[]
        {
            new TraceLevelSetting(nameof(TraceLevelSetting), MessageTraceLevel.Verbose),
            new IsUpdateServerEnabledSetting(true),
            new IncludePreReleasesSetting(true),
            new WebApiBaseUrlSetting(new Uri("https://api.rubberduckvba.com/api/v1")),
        };
    }
}
