using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Rubberduck.SettingsProvider.Model.Tools
{
    public abstract record class ToolWindowSettings : TypedSettingGroup
    {
        protected static readonly RubberduckSetting[] DefaultSettings =
            new RubberduckSetting[]
            {
                new ShowToolWindowOnStartupSetting(),
                new AutoHideToolWindowSetting(),
                new DefaultToolWindowLocationSetting(),
            };

        public ToolWindowSettings() { }

        public ToolWindowSettings(IDictionary<string, RubberduckSetting> settings) : this()
        {
            var defaultKeys = DefaultSettings.Select(e => e.Key).ToHashSet();
            Value = settings.Where(e => defaultKeys.Contains(e.Key));
        }

        [JsonIgnore]
        public bool ShowOnStartup { get; } = false;

        [JsonIgnore]
        public bool AutoHide { get; } = true;

        [JsonIgnore]
        public DockingLocation DefaultLocation { get; }
    }
}
