using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Rubberduck.SettingsProvider.Model
{
    public abstract record class SettingGroup : RubberduckSetting<IRubberduckSetting[]>
    {
        protected SettingGroup(string name, IEnumerable<IRubberduckSetting> settings, IEnumerable<IRubberduckSetting> defaults)
            : base(name, settings.ToArray(), SettingDataType.ObjectSetting, defaults.ToArray())
        {
            _map = Value.ToDictionary(e => e.GetType());
        }

        private readonly IDictionary<Type, IRubberduckSetting> _map;
        public TSetting GetSetting<TSetting>() where TSetting : IRubberduckSetting => (TSetting)_map[typeof(TSetting)];
        public IRubberduckSetting GetSetting(Type type) => _map[type];
    }
}
