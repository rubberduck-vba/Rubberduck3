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
        // TODO localize
        private static readonly string _description = "Configures the update server settings.";
        public static RubberduckSetting[] DefaultSettings = new RubberduckSetting[]
        {
            new TraceLevelSetting(nameof(TraceLevelSetting), MessageTraceLevel.Verbose),
            new IsUpdateServerEnabledSetting(true),
            new IncludePreReleasesSetting(true),
            new WebApiBaseUrlSetting(new Uri("https://api.rubberduckvba.com/api/v1")),
        };

        public static UpdateServerSettingsGroup Default { get; } = new(DefaultSettings);
        UpdateServerSettingsGroup IDefaultSettingsProvider<UpdateServerSettingsGroup>.Default => Default;

        public UpdateServerSettingsGroup() : base(nameof(UpdateServerSettingsGroup), _description) { }

        public UpdateServerSettingsGroup(UpdateServerSettingsGroup original, IEnumerable<RubberduckSetting>? settings = null)
            : base(original)
        {
            Settings = settings ?? DefaultSettings;
        }

        public UpdateServerSettingsGroup(IEnumerable<RubberduckSetting> settings)
            : base(nameof(UpdateServerSettingsGroup), _description)
        {
            Settings = settings;
        }

        public MessageTraceLevel TraceLevel => Enum.Parse<MessageTraceLevel>(Values[nameof(TraceLevelSetting)]);

        public bool IsEnabled => bool.Parse(Values[nameof(IsUpdateServerEnabledSetting)]);
        public bool IncludePreReleases => bool.Parse(Values[nameof(IncludePreReleasesSetting)]);
        public Uri RubberduckWebApiBaseUrl => new(Values[nameof(WebApiBaseUrlSetting)]);
    }
}
