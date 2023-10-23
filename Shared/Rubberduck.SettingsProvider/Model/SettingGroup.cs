using System.Collections.Generic;
using System.Linq;

namespace Rubberduck.SettingsProvider.Model
{
    public abstract record class SettingGroup : RubberduckSetting
    {
        protected SettingGroup(string name, string description)
            : base(SettingDataType.ObjectSetting, name, description)
        {
            Values = Settings.ToDictionary(setting => setting.Name, setting => setting.GetValue().ToString() ?? string.Empty);
        }

        protected abstract IEnumerable<RubberduckSetting> Settings { get; init; }
        public Dictionary<string, string> Values { get; init; }

        public sealed override object GetValue() => Settings;
    }
}
