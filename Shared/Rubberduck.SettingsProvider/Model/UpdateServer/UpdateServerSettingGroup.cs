using Rubberduck.InternalApi.Settings;
using System;
using System.Collections.Generic;

namespace Rubberduck.SettingsProvider.Model
{
    public interface IUpdateServerSettings
    {
        bool IsEnabled { get; }
        bool IncludePreReleases { get; }
        string RubberduckWebApiBaseUrl { get; }
    }

    public record class UpdateServerSettingGroup : SettingGroup, IDefaultSettingsProvider<UpdateServerSettingGroup>, IUpdateServerSettings
    {
        public static UpdateServerSettingGroup Default { get; } = new();
        UpdateServerSettingGroup IDefaultSettingsProvider<UpdateServerSettingGroup>.Default => Default;

        // TODO localize
        private static readonly string _description = "Configures the update server settings.";

        public UpdateServerSettingGroup() : base(nameof(UpdateServerSettingGroup), _description) { }

        public bool IsEnabled => bool.Parse(Values[nameof(IsUpdateServerEnabledSetting)]);
        public bool IncludePreReleases => bool.Parse(Values[nameof(IncludePreReleasesSetting)]);
        public Uri RubberduckWebApiBaseUrl => new(Values[nameof(WebApiBaseUrlSetting)]);

        protected override IEnumerable<RubberduckSetting> Settings { get; init; } = new RubberduckSetting[]
        {
            new IsUpdateServerEnabledSetting(true),
            new IncludePreReleasesSetting(true),
            new WebApiBaseUrlSetting(new Uri("https://api.rubberduckvba.com/api/v1")),
        };
    }
}
